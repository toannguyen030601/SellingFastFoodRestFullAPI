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
    public class OrderDetailsController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public OrderDetailsController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }
        [HttpGet]
        public IActionResult Index(int? id)
        {
            List<OrderDetail> listStudents = new List<OrderDetail>();

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "OrderDetails/GetOrderDetail/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var student = JsonConvert.DeserializeObject<OrderDetail>(data);

                    if (student != null)
                    {
                        listStudents.Add(student);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "OrderDetails/GetOrderDetails").Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    listStudents = JsonConvert.DeserializeObject<List<OrderDetail>>(data);
                }
                return View(listStudents);
            }

            return View(listStudents);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(OrderDetail stu)
        {
            string data = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PostAsync(_httpClient.BaseAddress + "OrderDetails/PostOrderDetail", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "OrderDetails/GetOrderDetail/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                OrderDetail stu = JsonConvert.DeserializeObject<OrderDetail>(data);

                return View(stu);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, OrderDetail stu)
        {
            string data = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PutAsync(_httpClient.BaseAddress + "OrderDetails/PutOrderDetail/" + id, content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(stu);
        }

        public IActionResult Delete(int id)
        {
            HttpResponseMessage stuMessage =
                _httpClient.DeleteAsync(_httpClient.BaseAddress + "OrderDetails/DeleteOrderDetail/" + id).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
