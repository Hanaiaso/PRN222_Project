using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System.Security.Claims;

namespace PRN_Project.Controllers
{
    [Authorize] // Bắt buộc có JWT token
    public class NotificationController : Controller
    {
        private readonly LmsDbContext _context;

        public NotificationController(LmsDbContext context)
        {
            _context = context;
        }

        // Lấy thông tin người dùng hiện tại từ JWT Claims
        private int CurrentAccountId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        private string CurrentRole =>
            User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        private bool IsAdmin() => CurrentRole == "Admin";
        private bool IsTeacher() => CurrentRole == "Teacher";
        private bool IsStudent() => CurrentRole == "Student";

        // ===== Danh sách tất cả thông báo (Admin-only) =====
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Index()
        {
            var notifications = _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .OrderByDescending(n => n.SentTime)
                .ToList();

            return View(notifications);
        }

        // ===== Xem chi tiết =====
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Details(int id)
        {
            var notification = _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .FirstOrDefault(n => n.NtId == id);

            if (notification == null)
                return NotFound();

            return View(notification);
        }

        // ===== Tạo thông báo mới =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Receivers = _context.Accounts
                .Where(a => a.Role == RoleType.Student && a.Status)
                .ToList();

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Notification notification, int[] selectedReceivers)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Receivers = _context.Accounts
                    .Where(a => a.Role == RoleType.Student && a.Status)
                    .ToList();
                return View(notification);
            }

            notification.SentTime = DateTime.Now;
            notification.SenderId = CurrentAccountId;

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            foreach (var receiverId in selectedReceivers)
            {
                var nr = new NotificationReceiver
                {
                    NtId = notification.NtId,
                    ReceiverId = receiverId,
                    IsRead = false
                };
                _context.NotificationReceivers.Add(nr);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ===== Chỉnh sửa =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
                return NotFound();

            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Notification notification)
        {
            if (!ModelState.IsValid)
                return View(notification);

            var existing = _context.Notifications.AsNoTracking().FirstOrDefault(n => n.NtId == notification.NtId);
            if (existing == null)
                return NotFound();

            notification.SenderId = existing.SenderId;
            notification.SentTime = existing.SentTime;

            _context.Update(notification);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ===== Xóa =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var notification = _context.Notifications
                .Include(n => n.Sender)
                .FirstOrDefault(n => n.NtId == id);

            if (notification == null)
                return NotFound();

            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int NtId)
        {
            var notification = _context.Notifications
                .Include(n => n.Receivers)
                .FirstOrDefault(n => n.NtId == NtId);

            if (notification == null)
                return NotFound();

            if (notification.Receivers != null)
                _context.NotificationReceivers.RemoveRange(notification.Receivers);

            _context.Notifications.Remove(notification);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ===== Danh sách thông báo cho người nhận =====
        [Authorize(Roles = "Student,Teacher,Admin")]
        public IActionResult MyNotifications()
        {
            var accountId = CurrentAccountId;

            var notifications = _context.NotificationReceivers
                .Include(nr => nr.Notification)
                    .ThenInclude(n => n.Sender)
                .Where(nr => nr.ReceiverId == accountId)
                .OrderByDescending(nr => nr.Notification.SentTime)
                .ToList();

            return View(notifications);
        }

        // ===== Đánh dấu đã đọc =====
        [Authorize(Roles = "Student,Teacher,Admin")]
        public IActionResult MarkAsRead(int id)
        {
            var receiver = _context.NotificationReceivers.Find(id);
            if (receiver == null)
                return NotFound();

            receiver.IsRead = true;
            _context.SaveChanges();

            return RedirectToAction(nameof(MyNotifications));
        }

        // ===== Xem chi tiết thông báo =====
        [Authorize(Roles = "Student,Teacher,Admin")]
        public IActionResult ViewNotification(int id)
        {
            var notification = _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .FirstOrDefault(n => n.NtId == id);

            if (notification == null)
                return NotFound();

            var accountId = CurrentAccountId;
            var receiver = _context.NotificationReceivers
                .FirstOrDefault(r => r.NtId == id && r.ReceiverId == accountId);

            if (receiver != null && !receiver.IsRead)
            {
                receiver.IsRead = true;
                _context.SaveChanges();
            }

            return View(notification);
        }
    }
}
