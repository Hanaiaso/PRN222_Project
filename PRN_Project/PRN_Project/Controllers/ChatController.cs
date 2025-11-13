using Microsoft.AspNetCore.Mvc;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin session
            var aid = HttpContext.Session.GetInt32("accountId");
            var userEmail = HttpContext.Session.GetString("userEmail");

            // Gọi Service để xử lý logic xác thực và lấy thông tin
            var userInfo = await _chatService.GetUserSessionInfoAsync(aid, userEmail);

            if (userInfo == null)
            {
                // Nếu chưa đăng nhập hoặc session hết hạn
                return RedirectToAction("Login", "Account");
            }

            // Truyền thông tin ra View
            ViewData["Title"] = "Trang giao tiếp";

            // Dùng thông tin từ Service
            ViewBag.StudentId = userInfo.AccountId;
            ViewBag.StudentName = userInfo.UserName;

            return View();
        }

        // Action xử lý khi người dùng nhập email và tìm người chat
        [HttpPost]
        public async Task<IActionResult> StartPrivateChat(string targetEmail)
        {
            var currentAccountId = HttpContext.Session.GetInt32("accountId");

            if (currentAccountId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập." });
            }

            // Gọi Service để tìm người nhận
            var targetUser = await _chatService.FindTargetUserAsync(targetEmail, currentAccountId.Value);

            if (targetUser == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng hoặc email không hợp lệ." });
            }

            // Lấy lịch sử chat
            var history = await _chatService.LoadPrivateChatHistoryAsync(currentAccountId.Value, targetUser.AId);

            // Lưu thông tin người nhận vào Session (RẤT QUAN TRỌNG cho SignalR)
            HttpContext.Session.SetInt32("TargetId", targetUser.AId);
            HttpContext.Session.SetString("TargetUserName", targetUser.Email);

            // Trả về ID và lịch sử
            return Json(new
            {
                success = true,
                targetUserId = targetUser.AId,
                targetUserName = targetUser.Email ?? targetUser.Email,
                history = history.Select(m => new {
                    senderId = m.SenderId,
                    senderName = m.Sender?.Email ?? m.Sender?.Email,
                    content = m.Content,
                    timestamp = m.Timestamp
                })
            });
        }
    }
}