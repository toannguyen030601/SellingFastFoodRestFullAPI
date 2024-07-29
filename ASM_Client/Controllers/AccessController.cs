using ASM_api.DB;
using ASM_api.Models;
using ASM_Client.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ASM_Client.Controllers
{
    public class AccessController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");
        private readonly net105AsmDBContext db;
        private readonly HttpClient _httpClient;
        private readonly INotyfService _notyf;

        public AccessController(net105AsmDBContext db, HttpClient httpClient, INotyfService notyf)
        {
            this.db = db;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
            _notyf = notyf;
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("EmailND") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Login(Customer customer)
        {
            if (HttpContext.Session.GetString("EmailND") == null)
            {
                var u = db.Customers.Where(x => x.Email.Equals(customer.Email) && x.Password.Equals(customer.Password)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("CustomerID", u.CustomerID.ToString());
                    HttpContext.Session.SetString("EmailND", u.Email.ToString());
                    HttpContext.Session.SetString("TenND", u.Name.ToString());
                    HttpContext.Session.SetString("SDTND", u.Phone.ToString());
                    HttpContext.Session.SetString("DiaChiND", u.Address.ToString());
                    _notyf.Success("Đăng nhập thành công!");
                    return RedirectToAction("Index", "Home");
                }
            }
            _notyf.Custom("Sai tài khoản hoặc mật khẩu",5,"red", "fa fa-times");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("CustomerID");
            HttpContext.Session.Remove("EmailND");
            HttpContext.Session.Remove("TenND");
            HttpContext.Session.Remove("SDTND");
            HttpContext.Session.Remove("DiaChiND");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Email))
            {
                return View();
            }
            var IsEmail = db.Customers.Where(u => u.Email == customer.Email).FirstOrDefault();
            if (IsEmail != null)
            {
                SendMail mail = new SendMail();
                string newPassword = mail.getPassword();
                mail.Send(customer.Email, newPassword, true);
                IsEmail.Password = newPassword;
                db.SaveChanges();

                // Lưu thông điệp vào TempData
                _notyf.Success("Mật khẩu mới đã được gửi vào Mail của bạn");

                return RedirectToAction("ForgotPassword", "Access");
            }
            else
            {
                // Nếu không tìm thấy email, thông báo lỗi
                _notyf.Custom("Tài khoản không tồn tại", 5, "red", "fa fa-times");
                return View();
            }
        }

        public IActionResult Taotaikhoannd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Taotaikhoannd(Customer customer)
        {

            if (ModelState.IsValid)
            {
                var existingEmail = db.Customers.FirstOrDefault(u => u.Email == customer.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "Email đã tồn tại.");
                    return View(customer);
                }

                db.Customers.Add(customer);
                db.SaveChanges();
                _notyf.Success("Tạo tài khoản thành công!");
                return RedirectToAction("Login", "Access");
            }

            // If model validation fails, return the same view with the model to display validation errors
            return View(customer);
        }

        public IActionResult UpdateHoSo()
        {
            var id = HttpContext.Session.GetString("CustomerID");
            var customer = db.Customers.FirstOrDefault(c => c.CustomerID == int.Parse(id));
            if (customer == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy khách hàng
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult UpdateHoSo(Customer customer)
        {
            int id = int.Parse(HttpContext.Session.GetString("CustomerID"));
            var mk = db.Customers.FirstOrDefault(p => p.CustomerID == id);
            customer = new Customer
            {
                CustomerID = id,
                Email = customer.Email,
                Address = customer.Address,
                Password = mk.Password,
                Name = customer.Name,
                Phone = customer.Phone,
            };

            string data = JsonConvert.SerializeObject(customer);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage stuMessage =
                _httpClient.PutAsync(_httpClient.BaseAddress + "Customers/PutCustomer/" + id, content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("CustomerID", customer.CustomerID.ToString());
                HttpContext.Session.SetString("EmailND", customer.Email.ToString());
                HttpContext.Session.SetString("TenND", customer.Name.ToString());
                HttpContext.Session.SetString("SDTND", customer.Phone.ToString());
                HttpContext.Session.SetString("DiaChiND", customer.Address.ToString());
                _notyf.Success("Sửa thành công!");
                return RedirectToAction("Index","Home");
            }
            return View(customer);
        }
    }
}
