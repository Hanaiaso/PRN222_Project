using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;

namespace PRN_Project.Controllers
{
    public class StudentController : Controller
    {
        private readonly LmsDbContext _context;

        public StudentController(LmsDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // Lấy accountId từ session
            var aid = HttpContext.Session.GetInt32("accountId");
            if (aid == null)
            {
                // Nếu chưa đăng nhập hoặc session hết hạn
                return RedirectToAction("Login", "Account");
            }

            // Tìm student theo AccountId
            var student = _context.Students.FirstOrDefault(s => s.AId == aid);
            if (student == null)
            {
                // Nếu không tìm thấy student
                return RedirectToAction("Error", "Home");
            }
            
            // Lưu SId vào Session
            HttpContext.Session.SetInt32("studentId", student.SId);
            HttpContext.Session.SetString("studentName", student.SName);

            // Truyền thông tin ra View
            ViewData["Title"] = "Trang chủ học sinh";
            ViewData["Student"] = student;
            ViewBag.StudentId = student.SId; // Dùng ViewBag để truyền ID
            ViewBag.StudentName = student.SName; // Dùng ViewBag để truyền Tên
            return View();
        }

        public IActionResult Exams() => View();

        public IActionResult Results() => View();

        public IActionResult Profile() => View();
    }
}
