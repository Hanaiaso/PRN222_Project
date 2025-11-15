using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels.Dashboard;

namespace PRN_Project.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;

        // private readonly IExamRepository _examRepo; 

        public TeacherController(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        // Action mặc định, có thể redirect về Dashboard
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // 1. Lấy AccountId từ Session (được set lúc Login trong AccountController)
            var accountId = HttpContext.Session.GetInt32("accountId");
            var role = HttpContext.Session.GetString("role");

            // 2. Kiểm tra đăng nhập và quyền hạn
            if (accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Teacher")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            // 3. Gọi Repository để lấy số liệu thống kê
            var classStats = await _dashboardRepo.GetTeacherClassStatsAsync(accountId.Value);

            // 4. Đóng gói vào ViewModel
            var viewModel = new TeacherDashboardViewModel
            {
                ClassStats = classStats
            };

            // 5. Trả về View (Views/Teacher/Dashboard.cshtml)
            return View(viewModel);
        }

    }
}