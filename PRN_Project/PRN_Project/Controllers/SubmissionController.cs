using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class SubmissionController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly IPostService _postService;

        public SubmissionController(ISubmissionService submissionService, IPostService postService)
        {
            _submissionService = submissionService;
            _postService = postService;
        }

        // === HỌC SINH NỘP BÀI TẬP ===
        [HttpPost]
        [Authorize(Roles = "Student")] // Chỉ học sinh mới được nộp bài
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int postId, string content)
        {
            // Lấy Post để lấy ClassroomId (cần cho redirect)
            var assignmentPost = await _postService.GetPostByIdAsync(postId);
            var targetClassroomId = assignmentPost?.ClassroomId ?? 0;

            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung bài làm.";
                if (targetClassroomId == 0)
                {
                    return RedirectToAction("Index", "Classroom");
                }
                return RedirectToAction("Details", "Classroom", new { id = targetClassroomId, tab = "classwork" });
            }

            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            
            var success = await _submissionService.SubmitAssignmentAsync(accountId, postId, content);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Đã nộp bài thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể nộp bài. Có thể bạn đã nộp rồi hoặc bài tập không tồn tại.";
            }

            if (targetClassroomId == 0)
            {
                return RedirectToAction("Index", "Classroom");
            }
            
            return RedirectToAction("Details", "Classroom", new { id = targetClassroomId, tab = "classwork" });
        }

        // === HỌC SINH CẬP NHẬT BÀI NỘP ===
        [HttpPost]
        [Authorize(Roles = "Student")] // Chỉ học sinh mới được sửa bài
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int postId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung bài làm.";
                var assignmentPost = await _postService.GetPostByIdAsync(postId);
                var targetClassroomId = assignmentPost?.ClassroomId ?? 0;
                if (targetClassroomId == 0)
                {
                    return RedirectToAction("Index", "Classroom");
                }
                return RedirectToAction("Details", "Classroom", new { id = targetClassroomId, tab = "classwork" });
            }

            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            
            var success = await _submissionService.UpdateSubmissionAsync(accountId, postId, content);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Đã cập nhật bài nộp thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật bài. Có thể bạn chưa nộp bài, đã quá hạn, hoặc bài tập không tồn tại.";
            }

            // Lấy ClassroomId từ Post để redirect
            var post = await _postService.GetPostByIdAsync(postId);
            var classroomId = post?.ClassroomId ?? 0;
            
            if (classroomId == 0)
            {
                return RedirectToAction("Index", "Classroom");
            }
            
            return RedirectToAction("Details", "Classroom", new { id = classroomId, tab = "classwork" });
        }
    }
}

