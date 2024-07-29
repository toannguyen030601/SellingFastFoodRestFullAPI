using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM_api.DB;
using ASM_api.Models;
using Newtonsoft.Json;
using System.Text;
using ASM_Client.Models;
using Microsoft.Extensions.Hosting;
using ASM_api.Services;
using X.PagedList;

namespace ASM_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/{controller}/{action}")]
    [Authentication]
    public class ProductsController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment environment;
        public ProductsController(HttpClient httpClient, IWebHostEnvironment environment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
            this.environment = environment;
        }
        [HttpGet]
        public IActionResult Index(int? id, string? Tensp, int? page)
        {
            List<Product> listStudents = new List<Product>();

            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Set ViewBag values to keep the state for pagination
            ViewBag.CurrentSearch = Tensp;

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProduct/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var student = JsonConvert.DeserializeObject<Product>(data);

                    if (student != null)
                    {
                        listStudents.Add(student);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "ProductClient/GetAllProduct?search=" + Tensp).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    listStudents = JsonConvert.DeserializeObject<List<Product>>(data);
                }
            }

            PagedList<Product> list = new PagedList<Product>(listStudents, pageNumber, pageSize);

            return View(list);
        }


        [HttpGet]
        public IActionResult Create()
        {
            List<Category> categories = LoadCategory();
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");
            List<Supplier> suppliers = LoadSupplier();
            ViewBag.SupplierID = new SelectList(suppliers, "SupplierID", "SupplierName");
            return View();
        }



        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = "";
            if (productDto.FileHinhAnh != null)
            {
                newFileName = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + Path.GetExtension(productDto.FileHinhAnh.FileName);
                string imgFullFath = environment.WebRootPath + "/images/products/" + newFileName;
                using (var stream = System.IO.File.Create(imgFullFath))
                {
                    productDto.FileHinhAnh.CopyTo(stream);
                }
            }

            Product product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryID = productDto.CategoryID,
                Availability = productDto.Availability,
                SupplierID = productDto.SupplierID,
                Hinhanh = newFileName
            };

            string productData = JsonConvert.SerializeObject(product);
            StringContent productContent = new StringContent(productData, Encoding.UTF8, "application/json");

            HttpResponseMessage postMessage = _httpClient.PostAsync(_httpClient.BaseAddress + "Products/PostProduct", productContent).Result;

            if (postMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(productDto);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            List<Category> categories = LoadCategory();
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");
            List<Supplier> suppliers = LoadSupplier();
            ViewBag.SupplierID = new SelectList(suppliers, "SupplierID", "SupplierName");


            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProduct/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                Product product = JsonConvert.DeserializeObject<Product>(data);

                var productDto = new ProductDto()
                {
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryID = product.CategoryID,
                    Availability = product.Availability,
                    SupplierID = product.SupplierID
                };

                ViewData["ProductID"] = product.ProductID;
                ViewData["Hinhanh"] = product.Hinhanh;

                return View(productDto);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProduct/" + id).Result;
            string data = resMessage.Content.ReadAsStringAsync().Result;
            Product product = JsonConvert.DeserializeObject<Product>(data);

            if (!ModelState.IsValid)
            {
                ViewData["ProductID"] = product.ProductID;
                ViewData["Hinhanh"] = product.Hinhanh;
                return View(productDto);
            }

            if (resMessage.IsSuccessStatusCode)
            {
                string newFileName = product.Hinhanh; // Keep the old image file name by default

                if (productDto.FileHinhAnh != null)
                {
                    // Create new file name and save the new image
                    newFileName = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + Path.GetExtension(productDto.FileHinhAnh.FileName);
                    string imgFullPath = Path.Combine(environment.WebRootPath, "images/products", newFileName);

                    using (var stream = System.IO.File.Create(imgFullPath))
                    {
                        productDto.FileHinhAnh.CopyTo(stream);
                    }

                    // Delete the old image file if it exists
                    if (!string.IsNullOrEmpty(product.Hinhanh))
                    {
                        string oldImgFullPath = Path.Combine(environment.WebRootPath, "images/products", product.Hinhanh);
                        if (System.IO.File.Exists(oldImgFullPath))
                        {
                            System.IO.File.Delete(oldImgFullPath);
                        }
                    }
                }

                // Update the product properties
                product.ProductName = productDto.ProductName;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.CategoryID = productDto.CategoryID;
                product.Availability = productDto.Availability;
                product.SupplierID = productDto.SupplierID;
                product.Hinhanh = newFileName; // Set the image file name, either old or new

                string productData = JsonConvert.SerializeObject(product);
                StringContent productContent = new StringContent(productData, Encoding.UTF8, "application/json");

                HttpResponseMessage putMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "Products/PutProduct/" + id, productContent).Result;

                if (putMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(productDto);
        }

        public IActionResult Delete(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProduct/" + id).Result;
            string data = resMessage.Content.ReadAsStringAsync().Result;
            Product product = JsonConvert.DeserializeObject<Product>(data);

            if (product == null)
            {
                return NotFound();
            }

            // Xóa hình ảnh từ thư mục lưu trữ
            if (!string.IsNullOrEmpty(product.Hinhanh))
            {
                string imgFullPath = environment.WebRootPath + "/images/products/" + product.Hinhanh;
                if (System.IO.File.Exists(imgFullPath))
                {
                    System.IO.File.Delete(imgFullPath);
                }
            }

            // Gửi yêu cầu DELETE đến API để xóa sản phẩm
            HttpResponseMessage deleteMessage = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Products/DeleteProduct/" + id).Result;

            if (deleteMessage.IsSuccessStatusCode)
            {
                // Nếu xóa thành công, chuyển hướng đến trang Index
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý khi không thể xóa sản phẩm (ví dụ: hiển thị thông báo lỗi)
                return View("Error");
            }
        }

        private List<Supplier> LoadSupplier()
        {
            List<Supplier> employees = new List<Supplier>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Suppliers/GetSuppliers").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<Supplier>>(data);
            }

            return employees;
        }

        private List<Category> LoadCategory()
        {
            List<Category> employees = new List<Category>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Categories/GetCategories").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<Category>>(data);
            }

            return employees;
        }

        public IActionResult ToggleAvailability(int id)
        {
            // Lấy sản phẩm từ ID
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProduct/" + id).Result;
            string data = resMessage.Content.ReadAsStringAsync().Result;
            Product product = JsonConvert.DeserializeObject<Product>(data);

            if (product == null)
            {
                return NotFound();
            }

            // Thay đổi trạng thái availability
            if (product.Availability == "Còn hàng")
            {
                product.Availability = "Hết hàng";
            }
            else
            {
                product.Availability = "Còn hàng";
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            string productData = JsonConvert.SerializeObject(product);
            StringContent productContent = new StringContent(productData, Encoding.UTF8, "application/json");

            HttpResponseMessage putMessage = _httpClient.PutAsync(_httpClient.BaseAddress + "Products/PutProduct/" + id, productContent).Result;

            if (!putMessage.IsSuccessStatusCode)
            {
                // Xử lý khi không thể cập nhật sản phẩm
                // Ví dụ: return BadRequest();
            }

            // Chuyển hướng lại đến trang Index
            return RedirectToAction(nameof(Index));
        }
    }
}
