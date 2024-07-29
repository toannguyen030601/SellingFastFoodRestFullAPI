using ASM_api.DB;
using ASM_api.Models;
using ASM_Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/{controller}/{action}")]
    public class AccessAdminController : Controller
    {
        private readonly net105AsmDBContext _context;

        public AccessAdminController(net105AsmDBContext context)
        {
            _context = context;
        }
        public IActionResult LoginAdmin()
        {
            if (HttpContext.Session.GetString("EmailEmployee") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "HomeAdmin");
            }
        }

        [HttpPost]
        public IActionResult LoginAdmin(Employee employee)
        {
            if (HttpContext.Session.GetString("EmailEmployee") == null)
            {
                var u = _context.Employees.Where(x => x.Email.Equals(employee.Email) && x.Password.Equals(employee.Password)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("IDEmployee", u.EmployeeID.ToString());
                    HttpContext.Session.SetString("EmailEmployee", u.Email.ToString());
                    HttpContext.Session.SetString("TenEmployee", u.Name.ToString());
                    return RedirectToAction("Index", "HomeAdmin");
                }
            }
            return View();
        }

        public IActionResult LogoutAdmin()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("EmailEmployee");
            HttpContext.Session.Remove("TenEmployee");
            return RedirectToAction("Index", "HomeAdmin");
        }

        public IActionResult ForgotPasswordAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPasswordAdmin(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Email))
            {
                TempData["Message"] = "Please enter your email.";
                return View();
            }
            var IsEmail = _context.Employees.Where(u => u.Email == employee.Email).FirstOrDefault();
            if (IsEmail != null)
            {
                SendMail mail = new SendMail();
                string newPassword = mail.getPassword();
                mail.Send(employee.Email, newPassword, true);
                IsEmail.Password = newPassword;
                _context.SaveChanges();

                // Lưu thông điệp vào TempData
                TempData["Message"] = "Check your email for the new password.";

                return View();
            }
            else
            {
                // Nếu không tìm thấy email, thông báo lỗi
                TempData["Message"] = "Email not found.";
                return View();
            }
        }
    }
}
