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
    public class EmployeesController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7041/api/");

        private readonly HttpClient _httpClient;
        public EmployeesController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUrl;
        }
        [HttpGet]
        public IActionResult Index(int? id)
        {
            List<Employee> listStudents = new List<Employee>();

            if (id.HasValue)
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Employees/GetEmployee/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    var student = JsonConvert.DeserializeObject<Employee>(data);

                    if (student != null)
                    {
                        listStudents.Add(student);
                    }
                }
            }
            else
            {
                HttpResponseMessage stuMessage =
                    _httpClient.GetAsync(_httpClient.BaseAddress + "Employees/GetEmployees").Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    string data = stuMessage.Content.ReadAsStringAsync().Result;
                    listStudents = JsonConvert.DeserializeObject<List<Employee>>(data);
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
        public IActionResult Create(Employee stu)
        {
            
            string data = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PostAsync(_httpClient.BaseAddress + "Employees/PostEmployee", content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resMessage = _httpClient.GetAsync(_httpClient.BaseAddress + "Employees/GetEmployee/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string data = resMessage.Content.ReadAsStringAsync().Result;
                Employee stu = JsonConvert.DeserializeObject<Employee>(data);

                return View(stu);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(int id, Employee stu)
        {
            string data = JsonConvert.SerializeObject(stu);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage stuMessage =
                _httpClient.PutAsync(_httpClient.BaseAddress + "Employees/PutEmployee/" + id, content).Result;

            if (stuMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(stu);
        }

        public IActionResult Delete(int id)
        {
            var isId = HttpContext.Session.GetString("IDEmployee");
            if (id != int.Parse(isId))
            {
                HttpResponseMessage stuMessage =
                _httpClient.DeleteAsync(_httpClient.BaseAddress + "Employees/DeleteEmployee/" + id).Result;

                if (stuMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
