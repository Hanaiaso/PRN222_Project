using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Trang chủ học sinh";
            return View();
        }

    }
}
