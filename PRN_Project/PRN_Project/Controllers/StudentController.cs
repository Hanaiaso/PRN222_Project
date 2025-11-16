using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Services.Interfaces; // THAY ĐỔI
// Bỏ using DbContext, EFCore, ViewModels, Json

namespace PRN_Project.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        // === THAY ĐỔI 1: Inject Service ===
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var aid = HttpContext.Session.GetInt32("accountId");
            if (aid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // === THAY ĐỔI 2: Gọi Service ===
            var student = await _studentService.GetStudentByAccountIdAsync(aid.Value);
            if (student == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Controller vẫn quản lý Session
            HttpContext.Session.SetInt32("studentId", student.SId);
            HttpContext.Session.SetString("studentName", student.SName);
            int? studentId = HttpContext.Session.GetInt32("studentId");

            // Truyền thông tin ra View
            ViewData["Title"] = "Trang chủ học sinh";
            ViewData["Student"] = student;
            ViewBag.StudentId = student.SId;
            ViewBag.StudentName = student.SName;
            var viewModel = await _studentService.GetStudentProgressReportAsync(studentId.Value);

            if (viewModel == null)
            {
                return NotFound($"Không tìm thấy học sinh với ID = {studentId}");
            }

            return View("StudentProgress",viewModel);
        }



        // Các action đơn giản này có thể giữ nguyên
        public IActionResult Exams() => View();
        public IActionResult Results() => View();
        public IActionResult Profile() => View();
    }
}