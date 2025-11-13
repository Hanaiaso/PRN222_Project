using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
