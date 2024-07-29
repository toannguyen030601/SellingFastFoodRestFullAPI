using ASM_api.Models;
using ASM_api.Services;
using ASM_Client.Models;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using X.PagedList;

namespace ASM_Client.Controllers
{
    public class HomeController : Controller
    {

        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public HomeController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }
        public IActionResult Index(string search, int? danhmuc, int? page)
        {
            List<Product> products = new List<Product>();

            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            ViewBag.CurrentCategory = danhmuc;
            ViewBag.Search = search;

            if (danhmuc.HasValue)
            {
                HttpResponseMessage responseCategory = _httpClient.GetAsync(_httpClient.BaseAddress + "ProductClient/GetProductsByCategory/" + danhmuc.Value).Result;
                if (responseCategory.IsSuccessStatusCode)
                {
                    var content = responseCategory.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<Product>>(content);
                }
            }
            else
            {
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "ProductClient/GetAllProduct?search=" + search).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<Product>>(content);
                }
            }

            ViewBag.Category = GetCategories();

            PagedList<Product> list = new PagedList<Product>(products, pageNumber, pageSize);
            return View(list);
        }


        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "CategoryClient/GetAllCategory").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<Category>>(content);
            }
            return categories;
        }

        public IActionResult Detail(int id, Feedback feedback)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProduct/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                var productJsonString = resMessage.Content.ReadAsStringAsync().Result;
                var product = JsonConvert.DeserializeObject<Product>(productJsonString);

                HttpResponseMessage responseLienquan = _httpClient.GetAsync(_httpClient.BaseAddress + "ProductClient/GetProductsByCategory/" + product.CategoryID).Result;
                if (responseLienquan.IsSuccessStatusCode)
                {
                    var contentlienquan = responseLienquan.Content.ReadAsStringAsync().Result;
                    var productsLienquan = JsonConvert.DeserializeObject<List<Product>>(contentlienquan);
                    ViewBag.RelatedProducts = productsLienquan;
                }

                HttpResponseMessage feedbackResponse = _httpClient.GetAsync(_httpClient.BaseAddress + "Feedbacks/GetFeedbacks").Result;
                if (feedbackResponse.IsSuccessStatusCode)
                {
                    var feedbackContent = feedbackResponse.Content.ReadAsStringAsync().Result;
                    var feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(feedbackContent);
                    ViewBag.Feedbacks = feedbacks;
                }

                if(ModelState.IsValid)
                {
                    var customerIdString = HttpContext.Session.GetString("CustomerID");

                    if (!string.IsNullOrEmpty(customerIdString))
                    {
                        feedback.CustomerID = int.Parse(customerIdString);
                        string postData = JsonConvert.SerializeObject(feedback);
                        StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");

                        HttpResponseMessage stuMessage =
                        _httpClient.PostAsync(_httpClient.BaseAddress + "Feedbacks/PostFeedback", content).Result;

                        if (stuMessage.IsSuccessStatusCode)
                        {
                            var feedbackContent2 = feedbackResponse.Content.ReadAsStringAsync().Result;
                            var feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(feedbackContent2);
                            ViewBag.Feedbacks = feedbacks;
                            return RedirectToAction("Detail", new { id = id });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Access");
                    }
                }
                return View(product);
            }
            else
            {
                // Xử lý trường hợp API không trả về dữ liệu thành công
                // Ví dụ: return NotFound(); để hiển thị trang 404
                return NotFound();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
