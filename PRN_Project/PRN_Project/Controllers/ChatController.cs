using Microsoft.AspNetCore.Mvc;
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels;

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

        public async Task<IActionResult> Community()
        {
            // ... (Lấy và kiểm tra Session tương tự như Index)
            var aid = HttpContext.Session.GetInt32("accountId");
            var userEmail = HttpContext.Session.GetString("userEmail");
            var userInfo = await _chatService.GetUserSessionInfoAsync(aid, userEmail);

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Chat Cộng đồng";
            ViewBag.StudentId = userInfo.AccountId;
            ViewBag.StudentName = userInfo.UserName;

            return View();
        }

        // Action hiển thị danh sách nhóm mà người dùng là thành viên
        public async Task<IActionResult> GroupList()
        {
            var aid = HttpContext.Session.GetInt32("accountId");
            if (aid == null) return RedirectToAction("Login", "Account");

            var groups = await _chatService.GetUserGroupsAsync(aid.Value);
            ViewData["Title"] = "Danh sách Nhóm Chat";
            return View(groups);
        }

        // Action xử lý tạo nhóm
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request) // ✅ ĐÃ SỬA LỖI
        {
            var creatorId = HttpContext.Session.GetInt32("accountId");

            if (creatorId == null) return Unauthorized();

            // SỬ DỤNG DỮ LIỆU TỪ request.GroupName và request.MemberEmails
            if (string.IsNullOrWhiteSpace(request.GroupName))
                return Json(new { success = false, message = "Tên nhóm không được trống." });

            // Trích xuất dữ liệu để sử dụng trong Service
            string groupName = request.GroupName;
            List<string> memberEmails = request.MemberEmails ?? new List<string>();


            // Gọi Service để tạo nhóm và thêm thành viên
            var group = await _chatService.CreateGroupWithMembersAsync(groupName, creatorId.Value, memberEmails);

            if (group == null)
                return Json(new { success = false, message = "Không thể tạo nhóm. (Lỗi Service)" });

            return Json(new { success = true, groupId = group.GroupId, groupName = group.Name });
        }

        // Action để vào phòng chat nhóm cụ thể
        public async Task<IActionResult> GroupChat(int groupId)
        {
            var aid = HttpContext.Session.GetInt32("accountId");
            if (aid == null) return RedirectToAction("Login", "Account");

            // Kiểm tra người dùng có thuộc nhóm không
            var group = await _chatService.GetGroupDetailsAsync(groupId, aid.Value);

            if (group == null)
            {
                TempData["Error"] = "Bạn không có quyền truy cập nhóm chat này.";
                return RedirectToAction("GroupList");
            }

            // Lấy thông tin người gửi (dùng lại logic từ Index)
            var userEmail = HttpContext.Session.GetString("userEmail");
            var userInfo = await _chatService.GetUserSessionInfoAsync(aid, userEmail);

            ViewData["Title"] = $"Nhóm: {group.Name}";
            ViewBag.GroupId = groupId;
            ViewBag.StudentId = userInfo?.AccountId;
            ViewBag.StudentName = userInfo?.UserName;

            return View(group);
        }
    }
}