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
    public class CustomersController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public CustomersController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }
        [HttpGet]
        public IActionResult Index(int? id)
        {
            List<Customer> customers = new List<Customer>();

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Customers/GetCustomer/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var a = JsonConvert.DeserializeObject<Customer>(data);

                    if (a != null)
                    {
                        customers.Add(a);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Customers/GetCustomers").Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    customers = JsonConvert.DeserializeObject<List<Customer>>(data);
                }
                return View(customers);
            }

            return View(customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            string data = JsonConvert.SerializeObject(customer);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PostAsync(_httpClient.BaseAddress + "Customers/PostCustomer", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Customers/GetCustomer/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                Customer customer = JsonConvert.DeserializeObject<Customer>(data);

                return View(customer);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, Customer customer)
        {
            string data = JsonConvert.SerializeObject(customer);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PutAsync(_httpClient.BaseAddress + "Customers/PutCustomer/" + id, content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        public IActionResult Delete(int id)
        {
            HttpResponseMessage stuMessage =
                _httpClient.DeleteAsync(_httpClient.BaseAddress + "Customers/DeleteCustomer/" + id).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
