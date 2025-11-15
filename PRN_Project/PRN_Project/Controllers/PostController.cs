using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PRN_Project.Services.Interfaces;
using PRN_Project.Models;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IClassroomService _classroomService;

        public PostController(IPostService postService, IClassroomService classroomService)
        {
            _postService = postService;
            _classroomService = classroomService;
        }

        // === TẠO BÀI ĐĂNG MỚI (THÔNG BÁO HOẶC BÀI TẬP) ===
        [HttpPost]
        [Authorize(Roles = "Teacher")] // Chỉ giáo viên mới được đăng bài
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int classroomId, string title, string content, string postType, DateTime? dueDate)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tiêu đề và nội dung.";
                return RedirectToAction("Details", "Classroom", new { id = classroomId, tab = "stream" });
            }

            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            
            // Kiểm tra người dùng có trong lớp không
            var classroom = await _classroomService.GetClassByIdAsync(classroomId);
            if (classroom == null)
            {
                TempData["ErrorMessage"] = "Lớp học không tồn tại.";
                return RedirectToAction("Index", "Classroom");
            }

            // Validate PostType
            if (postType != "Announcement" && postType != "Assignment")
            {
                postType = "Announcement"; // Mặc định là thông báo
            }

            // Nếu là Assignment thì phải có DueDate
            if (postType == "Assignment" && !dueDate.HasValue)
            {
                TempData["ErrorMessage"] = "Bài tập phải có hạn nộp.";
                return RedirectToAction("Details", "Classroom", new { id = classroomId, tab = "stream" });
            }

            await _postService.CreatePostAsync(classroomId, accountId, title, content, postType, dueDate);

            TempData["SuccessMessage"] = postType == "Assignment" ? "Đã đăng bài tập thành công!" : "Đã đăng thông báo thành công!";
            return RedirectToAction("Details", "Classroom", new { id = classroomId, tab = "stream" });
        }
    }
}

