using Microsoft.AspNetCore.Mvc;
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels;
using System.Threading.Tasks;

namespace PRN_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        private int? GetCurrentAccountId()
        {
            return HttpContext.Session.GetInt32("accountId");
        }

        private string? GetCurrentRole()
        {
            return HttpContext.Session.GetString("role");
        }

        // [GET] /Profile/ShowProfile
        [HttpGet]
        public async Task<IActionResult> ShowProfile()
        {
            int? accountId = GetCurrentAccountId();
            string? role = GetCurrentRole();

            if (accountId == null || role == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Gọi Service để lấy ViewModel
            var viewModel = await _profileService.GetProfileForViewAsync(accountId.Value, role);

            if (viewModel == null)
            {
                return NotFound("Không tìm thấy tài khoản");
            }

            ViewBag.Role = role;
            return View(viewModel);
        }

        // [GET] /Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            int? accountId = GetCurrentAccountId();
            string? role = GetCurrentRole();

            if (accountId == null || role == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Gọi Service để lấy ViewModel
            var viewModel = await _profileService.GetProfileForViewAsync(accountId.Value, role);

            if (viewModel == null)
            {
                return NotFound("Không tìm thấy tài khoản");
            }

            ViewBag.Role = role;
            return View(viewModel);
        }

        // [POST] /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            int? accountId = GetCurrentAccountId();
            string? role = GetCurrentRole();
            ViewBag.Role = role;

            if (accountId == null || role == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(model); // Có lỗi, trả về form
            }

            // Gọi Service để cập nhật
            var updateSuccess = await _profileService.UpdateProfileAsync(accountId.Value, role, model);

            if (!updateSuccess)
            {
                ModelState.AddModelError("", "Không thể lưu thay đổi. Hãy thử lại.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ShowProfile");
        }
    }

}