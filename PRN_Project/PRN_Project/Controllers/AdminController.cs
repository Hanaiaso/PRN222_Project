using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Trang chủ học sinh";
            return View();
        }
    }
}
