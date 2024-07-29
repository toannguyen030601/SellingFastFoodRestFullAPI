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

namespace ASM_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/{controller}/{action}")]

    public class FeedbacksController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public FeedbacksController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }
        [HttpGet]
        public IActionResult Index(int? id)
        {
            List<Feedback> listStudents = new List<Feedback>();

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Feedbacks/GetFeedback/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var student = JsonConvert.DeserializeObject<Feedback>(data);

                    if (student != null)
                    {
                        listStudents.Add(student);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Feedbacks/GetFeedbacks").Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    listStudents = JsonConvert.DeserializeObject<List<Feedback>>(data);
                }
                return View(listStudents);
            }

            return View(listStudents);
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<Customer> customers = new List<Customer>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Customers/GetCustomers").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                customers = JsonConvert.DeserializeObject<List<Customer>>(data);
            }

            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Feedback stu)
        {
            List<Customer> customers = new List<Customer>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Customers/GetCustomers").Result;
            if (response.IsSuccessStatusCode)
            {
                string responseData = response.Content.ReadAsStringAsync().Result;
                customers = JsonConvert.DeserializeObject<List<Customer>>(responseData);
            }

            if (customers == null)
            {
                // Handle the case where customers is null (e.g., log an error, return a specific view, etc.)
                // For now, let's assume returning an error view
                return View("Error");
            }

            int selectedCustomerID = stu.CustomerID;
            Customer selectedCustomer = customers.FirstOrDefault(c => c.CustomerID == selectedCustomerID);

            if (selectedCustomer == null)
            {
                ModelState.AddModelError("CustomerID", "Invalid customer selected.");
                return View(stu);
            }

            stu.CustomerID = selectedCustomerID;
            stu.CreatedAt = DateTime.Now;

            string postData = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PostAsync(_httpClient.BaseAddress + "Feedbacks/PostFeedback", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(stu);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Feedbacks/GetFeedback/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                Feedback stu = JsonConvert.DeserializeObject<Feedback>(data);

                // Load customers for dropdown
                List<Customer> customers = LoadCustomers();
                ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name", stu.CustomerID);

                return View(stu);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, Feedback stu)
        {
            // Load customers for dropdown
            List<Customer> customers = LoadCustomers();
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "Name", stu.CustomerID);

            if (id != stu.FeedbackID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(stu);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage stuMessage =
                    _httpClient.PutAsync(_httpClient.BaseAddress + "Feedbacks/PutFeedback/" + id, content).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(stu);
        }

        public IActionResult Delete(int id)
        {
            HttpResponseMessage stuMessage =
                _httpClient.DeleteAsync(_httpClient.BaseAddress + "Feedbacks/DeleteFeedback/" + id).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
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
    }
}
