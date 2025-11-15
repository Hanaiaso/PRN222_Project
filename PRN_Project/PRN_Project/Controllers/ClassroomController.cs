using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; 
using PRN_Project.Services.Interfaces;
using PRN_Project.Models;

namespace PRN_Project.Controllers
{
    [Authorize] 
    public class ClassroomController : Controller
    {
        private readonly IClassroomService _classroomService;
        private readonly ISubmissionService _submissionService;
        private readonly IPostService _postService;

        public ClassroomController(IClassroomService classroomService, ISubmissionService submissionService, IPostService postService)
        {
            _classroomService = classroomService;
            _submissionService = submissionService;
            _postService = postService;
        }

        // dashboard của lớp học
        public async Task<IActionResult> Index()
        {
            
            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // AccountId từ JWT token
            var userRole = User.FindFirstValue(ClaimTypes.Role); 

            // Service sẽ tự động chuyển đổi AccountId sang TeacherId/StudentId và lấy đúng lớp học
            var myClasses = await _classroomService.GetMyClassesAsync(accountId, userRole);

            return View(myClasses); 
        }

        // học sinh join lớp
        [HttpPost]
        [Authorize(Roles = "Student")] // check Role của học sinh
        public async Task<IActionResult> Join(string classCode)
        {
            if (string.IsNullOrEmpty(classCode))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập mã lớp.";
                return RedirectToAction("Index");
            }

            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Lấy AccountId
            var success = await _classroomService.JoinClassAsync(accountId, classCode);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã tham gia lớp học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Mã lớp không tồn tại hoặc bạn đã ở trong lớp này.";
            }

            return RedirectToAction("Index");
        }

        // === HÀNH ĐỘNG TẠO LỚP (CHO GIÁO VIÊN) ===
        [HttpPost]
        [Authorize(Roles = "Teacher")] // Chỉ giáo viên mới được Tạo
        public async Task<IActionResult> Create(string className, string description)
        {
            if (string.IsNullOrEmpty(className))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên lớp.";
                return RedirectToAction("Index");
            }

            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Lấy AccountId

            await _classroomService.CreateClassAsync(className, description, accountId);

            TempData["SuccessMessage"] = "Đã tạo lớp học thành công!";
            return RedirectToAction("Index");
        }

        // === TRANG CHI TIẾT LỚP HỌC ===
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroom = await _classroomService.GetClassByIdAsync(id.Value);
            if (classroom == null)
            {
                return NotFound();
            }

            // Lọc các bài tập (PostType = "Assignment") và sắp xếp theo thời gian
            var assignments = classroom.Posts?
                .Where(p => p.PostType?.Equals("Assignment", StringComparison.OrdinalIgnoreCase) == true)
                .OrderByDescending(p => p.CreateTime)
                .ToList() ?? new List<PRN_Project.Models.Post>();

            // Truyền dữ liệu đã lọc vào ViewBag
            ViewBag.Assignments = assignments;
            ViewBag.TotalStudents = classroom.Members?.Count ?? 0;

            // Nếu là học sinh, lấy thông tin bài nộp của họ để truyền vào ViewBag (cho cả Stream và Classwork)
            if (User.IsInRole("Student"))
            {
                var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                var studentSubmissions = new Dictionary<int, AssignmentSubmission?>();
                
                // Lấy submission cho tất cả Posts (không chỉ Assignment) để hiển thị ở Stream
                if (classroom.Posts != null)
                {
                    foreach (var post in classroom.Posts.Where(p => p.PostType == "Assignment"))
                    {
                        var submission = await _submissionService.GetMySubmissionAsync(accountId, post.PostId);
                        studentSubmissions[post.PostId] = submission;
                    }
                }
                
                ViewBag.StudentSubmissions = studentSubmissions;
            }

            return View(classroom);
        }

        // === GIÁO VIÊN XEM CHI TIẾT HỌC SINH ===
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> StudentDetail(int studentId, int classroomId)
        {
            // Kiểm tra học sinh có trong lớp không
            var classroom = await _classroomService.GetClassByIdAsync(classroomId);
            if (classroom == null)
            {
                return NotFound();
            }

            var student = classroom.Members?.FirstOrDefault(m => m.Student?.SId == studentId)?.Student;
            if (student == null)
            {
                return NotFound("Học sinh không có trong lớp này.");
            }

            ViewBag.ClassroomId = classroomId;

            return View(student);
        }
    }
}