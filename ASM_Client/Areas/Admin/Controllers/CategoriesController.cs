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

namespace ASM_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/{controller}/{action}")]
    [Authentication]
    public class CategoriesController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public CategoriesController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }

        [HttpGet]
        public IActionResult Index(int? id)
        {
            List<Category> categories = new List<Category>();

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Categories/GetCategory/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var student = JsonConvert.DeserializeObject<Category>(data);

                    if (student != null)
                    {
                        categories.Add(student);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Categories/GetCategories").Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    categories = JsonConvert.DeserializeObject<List<Category>>(data);
                }
                return View(categories);
            }

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            string data = JsonConvert.SerializeObject(category);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PostAsync(_httpClient.BaseAddress + "Categories/PostCategory", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Categories/GetCategory/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                Category category = JsonConvert.DeserializeObject<Category>(data);

                return View(category);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, Category category)
        {
            string data = JsonConvert.SerializeObject(category);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PutAsync(_httpClient.BaseAddress + "Categories/PutCategory/" + id, content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public IActionResult Delete(int id)
        {
            HttpResponseMessage stuMessage =
                _httpClient.DeleteAsync(_httpClient.BaseAddress + "Categories/DeleteCategory/" + id).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
