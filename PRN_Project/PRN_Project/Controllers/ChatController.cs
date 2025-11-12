using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;

namespace PRN_Project.Controllers
{
    public class ChatController : Controller
    {
        private readonly LmsDbContext _context;

        public ChatController(LmsDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Lấy accountId từ session
            var aid = HttpContext.Session.GetInt32("accountId");
            if (aid == null)
            {
                // Nếu chưa đăng nhập hoặc session hết hạn
                return RedirectToAction("Login", "Account");
            }

            // Truyền thông tin ra View
            ViewData["Title"] = "Trang giao tiếp";
        
            ViewBag.StudentId = HttpContext.Session.GetInt32("accountId"); ; // Dùng ViewBag để truyền ID
            ViewBag.StudentName = HttpContext.Session.GetString("userEmail"); // Dùng ViewBag để truyền Tên
            return View();
        }


    }
}
