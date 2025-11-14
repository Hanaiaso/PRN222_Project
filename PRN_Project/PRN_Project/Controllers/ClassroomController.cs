using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    public class ClassroomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
