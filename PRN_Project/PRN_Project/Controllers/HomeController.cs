using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            ViewBag.Email = email;
            ViewBag.Role = role;
            return View();
        }

        public IActionResult Error(int statusCode)
        {
            if (statusCode == 403)
                ViewBag.Message = "Bạn không có quyền truy cập trang này.";
            else if (statusCode == 401)
                ViewBag.Message = "Vui lòng đăng nhập để tiếp tục.";
            else
                ViewBag.Message = "Đã xảy ra lỗi.";

            return View();
        }
    }
}
