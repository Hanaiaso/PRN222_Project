using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
