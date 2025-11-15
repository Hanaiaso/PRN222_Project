using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Hubs;
using PRN_Project.Models;
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels;
using System.Security.Claims;

namespace PRN_Project.Controllers
{
    [Authorize] // Bắt buộc có JWT token
    public class NotificationController : Controller
    {
        private readonly INotificationService _service;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationController(INotificationService service, IHubContext<NotificationHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // Lấy thông tin người dùng hiện tại từ JWT Claims
        private int CurrentAccountId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        private string CurrentRole =>
            User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        private bool IsAdmin() => CurrentRole == "Admin";

        // ===== Danh sách tất cả thông báo (Admin-only) =====
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var notifications = await _service.GetAllAsync();
            return View(notifications);
        }

        // ===== Xem chi tiết =====
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var notification = await _service.GetDetailsAsync(id);

            if (notification == null)
                return NotFound();

            return View(notification);
        }

        // ===== Tạo thông báo mới =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification notification)
        {
            if (!ModelState.IsValid)
                return View(notification);

            await _service.CreateForAllAsync(notification, CurrentAccountId);

            return RedirectToAction(nameof(Index));
        }

        // ===== Chỉnh sửa =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var notification = await _service.GetDetailsAsync(id);
            if (notification == null)
                return NotFound();

            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Notification notification)
        {
            if (!ModelState.IsValid)
                return View(notification);

            await _service.UpdateAsync(notification);
            return RedirectToAction(nameof(Index));
        }

        // ===== Xóa =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _service.GetDetailsAsync(id);
            if (notification == null)
                return NotFound();

            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int NtId)
        {
            await _service.DeleteAsync(NtId);
            return RedirectToAction(nameof(Index));
        }

        // ===== Danh sách thông báo cho người nhận =====
        [Authorize(Roles = "Student,Teacher,Admin")]
        public async Task<IActionResult> MyNotifications()
        {
            var list = await _service.GetMyNotificationsAsync(CurrentAccountId);
            return View(list.OrderByDescending(x => x.Notification!.SentTime).ToList());
        }

        // ===== Đánh dấu đã đọc =====
        [Authorize(Roles = "Student,Teacher,Admin")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _service.MarkAsReadAsync(id);
            return RedirectToAction(nameof(MyNotifications));
        }

        // ===== Xem chi tiết thông báo =====
        [Authorize(Roles = "Student,Teacher,Admin")]
        public async Task<IActionResult> ViewNotification(int id)
        {
            var notification = await _service.GetDetailsAsync(id);

            if (notification == null)
                return NotFound();

            var otherNotifications = await _service.GetOtherNotificationsAsync(id);

            var vm = new NotificationDetailViewModel
            {
                CurrentNotification = notification,
                OtherNotifications = otherNotifications
            };

            var myList = await _service.GetMyNotificationsAsync(CurrentAccountId);
            var receiver = myList.FirstOrDefault(r => r.NtId == id);

            if (receiver != null && !receiver.IsRead)
                await _service.MarkAsReadAsync(receiver.NrId);

            return View(vm);
        }
    }
}
