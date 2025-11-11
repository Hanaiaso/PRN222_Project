using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index(string receiverId)
        {
            ViewBag.ReceiverId = receiverId;
            return View();
        }
    }
}
