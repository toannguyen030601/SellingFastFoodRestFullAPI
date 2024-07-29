using ASM_api.DB;
using ASM_api.Models;
using ASM_Client.Models;
using ASM_Client.Models.VNPAY;
using ASM_Client.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Text;

namespace ASM_Client.Controllers
{
    public class CartsController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");
        private readonly net105AsmDBContext context;
        private readonly HttpClient _httpClient;
        private readonly INotyfService _notyf;
        private readonly IVnPaySvc _vnPaySvc;

        public CartsController(net105AsmDBContext context, HttpClient httpClient, INotyfService noty, IVnPaySvc vnPaySvc)
        {
            this.context = context;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
            _notyf = noty;
            _vnPaySvc = vnPaySvc;
        }
       
        private int isExit(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].product.ProductID.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        public IActionResult Buy(int id)
        {
            ProductModel productModel = new ProductModel(context); // Pass the context to ProductModel

            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { product = productModel.Find(id), quantity = 1 });

                _notyf.Success("Sản phẩm đã được thêm vào giỏ hàng");
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExit(id);
                if (index != -1)
                {
                    cart[index].quantity++;
                }
                else
                {
                    cart.Add(new Item { product = productModel.Find(id), quantity = 1 });
                }
                _notyf.Success("Sản phẩm đã được thêm vào giỏ hàng");
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExit(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            if (cart != null && cart.Count > 0)
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(x => x.product.Price * x.quantity);
            }
            else
            {
                ViewBag.cart = new List<Item>();
                ViewBag.total = 0;
                ViewBag.Message = "Chưa có đơn hàng.";
            }

            return View();
        }

        public IActionResult DatHang(string paymentMethod)
        {
            var carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (carts == null || carts.Count == 0)
            {
                // Nếu giỏ hàng trống, chuyển hướng đến trang thông báo
                return RedirectToAction("Index");
            }

            if (HttpContext.Session.GetString("EmailND") != null)
            {
                if (paymentMethod == "vnpay")
                {
                    var tongtien = carts.Sum(x => x.product.Price * x.quantity);
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = (double)tongtien,
                        CreatedDate = DateTime.Now,
                        Description = "Thanh toán đơn hàng",
                        FullName = HttpContext.Session.GetString("TenND"),
                        OrderId = new Random().Next(1000,100000)
                    };
                    return Redirect(_vnPaySvc.CreatePaymentUrl(HttpContext, vnPayModel));
                }



                int customerid = int.Parse(HttpContext.Session.GetString("CustomerID"));
                Order order = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerID = customerid,
                    TotalAmount = carts.Sum(x => x.product.Price * x.quantity),
                    Status = "Chờ xử lí",
                    PaymentStatus = "Chưa thanh toán",
                    ModifiedDate = DateTime.Now
                };

                string data = JsonConvert.SerializeObject(order);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage stuMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "Orders/PostOrder", content).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    var createdOrder = JsonConvert.DeserializeObject<Order>(stuMessage.Content.ReadAsStringAsync().Result);

                    foreach (var cart in carts)
                    {
                        OrderDetail donHang = new OrderDetail
                        {
                            OrderID = createdOrder.OrderID,
                            ProductID = cart.product.ProductID,
                            Quantity = cart.quantity,
                            Price = cart.product.Price * cart.quantity
                        };

                        string detailData = JsonConvert.SerializeObject(donHang);
                        StringContent detailContent = new StringContent(detailData, Encoding.UTF8, "application/json");
                        HttpResponseMessage detailResponse = _httpClient.PostAsync(_httpClient.BaseAddress + "OrderDetails/PostOrderDetail", detailContent).Result;

                        if (!detailResponse.IsSuccessStatusCode)
                        {
                            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu chi tiết đơn hàng. Vui lòng thử lại.");
                            return View("Error");
                        }
                    }
                    _notyf.Success("Sản phẩm đã được đặt thành công!");
                    HttpContext.Session.Remove("cart");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu đơn hàng. Vui lòng thử lại.");
                    return View("Error");
                }
            }
            else
            {
                // Xử lý trường hợp khi người dùng chưa đăng nhập
                return RedirectToAction("Login", "Access");
            }
        }


        public IActionResult lichsumua()
        {
            var id = HttpContext.Session.GetString("CustomerID");

            var a = context.OrderDetails.Include(o => o.Order).Include(p => p.Product).ToList();
            var b = a.Where(x => x.Order.CustomerID == int.Parse(id)).ToList();

            

            return View(b);
        }

        public IActionResult XoaDonHang(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Orders/GetOrder/" + id).Result;
            string data = resMessage.Content.ReadAsStringAsync().Result;
            Order order = JsonConvert.DeserializeObject<Order>(data);

            if (order == null)
            {
                return NotFound();
            }

            // Thay đổi trạng thái availability
            if (order.Status == "Chờ xử lí")
            {
                order.Status = "Đã hủy";
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            string orderData = JsonConvert.SerializeObject(order);
            StringContent orderContent = new StringContent(orderData, Encoding.UTF8, "application/json");

            HttpResponseMessage putMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "Orders/PutOrder/" + id, orderContent).Result;

            if (!putMessage.IsSuccessStatusCode)
            {
                // Xử lý khi không thể cập nhật sản phẩm
                // Ví dụ: return BadRequest();
            }

            // Chuyển hướng lại đến trang Index
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PaymentCallBack()
        {
            var response = _vnPaySvc.PaymenteExecute(Request.Query);
            if(response == null || response.VnPayResponseCode != "00")
            {
                _notyf.Custom("Lỗi hoặc đã hủy thanh toán!",5,"red", "fa fa-times");
                return RedirectToAction(nameof(lichsumua));
            }


            var carts = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (carts == null || carts.Count == 0)
            {
                // Nếu giỏ hàng trống, chuyển hướng đến trang thông báo
                return RedirectToAction("Index");
            }

            int customerid = int.Parse(HttpContext.Session.GetString("CustomerID"));
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerID = customerid,
                TotalAmount = carts.Sum(x => x.product.Price * x.quantity),
                Status = "Chờ xử lí",
                PaymentStatus = "Đã thanh toán qua VnPay",
                ModifiedDate = DateTime.Now
            };

            string data = JsonConvert.SerializeObject(order);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage stuMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "Orders/PostOrder", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                var createdOrder = JsonConvert.DeserializeObject<Order>(stuMessage.Content.ReadAsStringAsync().Result);

                foreach (var cart in carts)
                {
                    OrderDetail donHang = new OrderDetail
                    {
                        OrderID = createdOrder.OrderID,
                        ProductID = cart.product.ProductID,
                        Quantity = cart.quantity,
                        Price = cart.product.Price * cart.quantity
                    };

                    string detailData = JsonConvert.SerializeObject(donHang);
                    StringContent detailContent = new StringContent(detailData, Encoding.UTF8, "application/json");
                    HttpResponseMessage detailResponse = _httpClient.PostAsync(_httpClient.BaseAddress + "OrderDetails/PostOrderDetail", detailContent).Result;

                    if (!detailResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu chi tiết đơn hàng. Vui lòng thử lại.");
                        return View("Error");
                    }
                }
                _notyf.Success("Thanh toán VnPay thành công!");
                HttpContext.Session.Remove("cart");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu đơn hàng. Vui lòng thử lại.");
                return View("Error");
            }
        }
    }
}
