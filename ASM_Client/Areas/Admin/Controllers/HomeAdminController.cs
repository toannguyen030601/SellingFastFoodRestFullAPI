
using ASM_api.DB;
using ASM_Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Home")]
    [Authentication]
    public class HomeAdminController : Controller
    {

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            /*ViewBag.slCustomers = _context.Customers.Count();
            ViewBag.slEmployees = _context.Employees.Count();
            ViewBag.TongThuNhap = _context.Orders.Sum(e => e.TotalAmount).ToString("#,###");
            ViewBag.slOrdersWait = _context.Orders.Where(e => e.Status == "Đợi xử lí").Count();*/
            return View();
        }
    }
}
