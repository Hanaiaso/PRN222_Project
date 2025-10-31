using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Trang chủ học sinh";
            return View();
        }

        public IActionResult Exams() => View();
        public IActionResult Results() => View();
        public IActionResult Profile() => View();
    }
}
