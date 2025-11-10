using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;


namespace PRN_Project.Controllers
{
    
    public class NotificationController : Controller
    {
        
        private readonly LmsDbContext _context;

        public NotificationController(LmsDbContext context)
        {
            _context = context;
        }

        private int? CurrentAccountId => HttpContext.Session.GetInt32("accountId");
        private string? CurrentRole => HttpContext.Session.GetString("role");
        private bool IsAdmin() => CurrentRole == "Admin";
        private bool IsTeacher() => CurrentRole == "Teacher";
        private bool IsStudent() => CurrentRole == "Student";


        //danh sach tat ca thong bao
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            var notifications = _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .OrderByDescending(n => n.SentTime)
                .ToList();
            return View(notifications);
        }
        //chi tiet thong bao
        public IActionResult Details(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            var notification = _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .FirstOrDefault(n => n.NtId == id);

            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }
        //Tao thong bao moi
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            ViewBag.Receivers = _context.Accounts
                .Where(a => a.Role == RoleType.Student && a.Status)
                .ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Notification notification, int[] selectedReceivers)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            if (ModelState.IsValid)
            {
                notification.SentTime = DateTime.Now;
                notification.SenderId = 1;
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
            ViewBag.Receivers = _context.Accounts
                .Where(a => a.Role == RoleType.Student && a.Status)
                .ToList();
            return View(notification);
        }
        //Chinh sua thong bao
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            var notification = _context.Notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Notification notification)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            if (ModelState.IsValid)
            {
                var existing = _context.Notifications.AsNoTracking().FirstOrDefault(n => n.NtId == notification.NtId);
                if (existing == null)
                {
                    return NotFound();
                }
                notification.SenderId = existing.SenderId;
                notification.SentTime = existing.SentTime;

                _context.Update(notification);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }
        //Xoa thong bao
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            var notification = _context.Notifications
            .Include(n => n.Sender)
            .FirstOrDefault(n => n.NtId == id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int NtId)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Home");
            var notification = _context.Notifications
            .Include(n => n.Receivers)
            .FirstOrDefault(n => n.NtId == NtId);

            if (notification == null)
            {
                return NotFound();
            }

            if (notification.Receivers != null)
                _context.NotificationReceivers.RemoveRange(notification.Receivers);

            _context.Notifications.Remove(notification);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        //Danh sach thong bao cho nguoi nhan
        public IActionResult MyNotifications()
        {
            int accountId = CurrentAccountId ?? 0; 

            var notifications = _context.NotificationReceivers
                .Include(nr => nr.Notification)
                    .ThenInclude(n => n.Sender)
                .Where(nr => nr.ReceiverId == accountId)
                .OrderByDescending(nr => nr.Notification.SentTime)
                .ToList();

            return View(notifications);
        }
        //danh dau da doc thong bao
        public IActionResult MarkAsRead(int id)
        {
            var receiver = _context.NotificationReceivers.Find(id);
            if (receiver == null)
            {
                return NotFound();
            }
            receiver.IsRead = true;
            _context.SaveChanges();

            return RedirectToAction(nameof(MyNotifications), new { accountId = receiver.ReceiverId });
        }

        public IActionResult ViewNotification(int id)
        {
            var notification = _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .FirstOrDefault(n => n.NtId == id);

            if (notification == null)
            {
                return NotFound();
            }

            int accountId = CurrentAccountId ?? 0; 
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
