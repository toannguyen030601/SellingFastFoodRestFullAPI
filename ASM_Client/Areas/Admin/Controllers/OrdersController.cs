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

    public class OrdersController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public OrdersController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }
        [HttpGet]
        public IActionResult Index(int? id)
        {
            List<Order> listStudents = new List<Order>();

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Orders/GetOrder/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var student = JsonConvert.DeserializeObject<Order>(data);

                    if (student != null)
                    {
                        listStudents.Add(student);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Orders/GetOrders").Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    listStudents = JsonConvert.DeserializeObject<List<Order>>(data);
                }
                return View(listStudents);
            }

            return View(listStudents);
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<Customer> customers = LoadCustomers();
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name");
            List<Employee> employees = LoadEmployee();
            ViewBag.EmployeeID = new SelectList(employees, "EmployeeID", "Name");
            List<Product> products = LoadProduct();
            ViewBag.ProductID = new SelectList(products, "ProductID", "ProductName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order stu)
        {
            string data = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PostAsync(_httpClient.BaseAddress + "Orders/PostOrder", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            List<Customer> customers = LoadCustomers();
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name", stu.CustomerID);

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Orders/GetOrder/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                Order stu = JsonConvert.DeserializeObject<Order>(data);

                List<Customer> customers = LoadCustomers();
                ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name",stu.CustomerID);

                return View(stu);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, Order stu)
        {
            List<Customer> customers = LoadCustomers();
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name", stu.CustomerID);


            string data = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PutAsync(_httpClient.BaseAddress + "Orders/PutOrder/" + id, content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(stu);
        }

        public IActionResult Delete(int id)
        {
            HttpResponseMessage stuMessage =
                _httpClient.DeleteAsync(_httpClient.BaseAddress + "Orders/DeleteOrder/" + id).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public async Task<List<SelectListItem>> GetProducts()
        {
            List<SelectListItem> products = new List<SelectListItem>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "Products");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                List<Product> productList = JsonConvert.DeserializeObject<List<Product>>(data);

                products = productList.Select(p => new SelectListItem { Value = p.ProductID.ToString(), Text = p.ProductName }).ToList();
            }

            return products;
        }
        private List<Customer> LoadCustomers()
        {
            List<Customer> customers = new List<Customer>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Customers/GetCustomers").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                customers = JsonConvert.DeserializeObject<List<Customer>>(data);
            }

            return customers;
        }
        private List<Product> LoadProduct()
        {
            List<Product> products = new List<Product>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Products/GetProducts").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(data);
            }

            return products;
        }

        private List<Employee> LoadEmployee()
        {
            List<Employee> employees = new List<Employee>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Employees/GetEmployees").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<Employee>>(data);
            }

            return employees;
        }


    }
}
