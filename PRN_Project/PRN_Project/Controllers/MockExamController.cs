using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Models.JsonModels;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class MockExamController : Controller
    {
        // Khai báo Service thay vì Context
        private readonly IMockExamService _examService;

        // Dependency Injection cho Service
        public MockExamController(IMockExamService examService)
        {
            _examService = examService;
        }

        // Step 1: chọn môn thi
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ChooseSubject()
        {
            var subjects = await _examService.GetSubjectsAsync();
            return View(subjects);
        }

        // Step 2: chọn bài thi
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ChooseExam(int subjectId)
        {
            var sid = HttpContext.Session.GetInt32("studentId");
            if (sid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Gọi Service để lấy dữ liệu
            var (exams, subject, doneExamIds) = await _examService.GetExamsForStudentAsync(subjectId, sid.Value);

            // Truyền vào ViewBag
            ViewBag.Subject = subject;
            ViewBag.DoneExams = doneExamIds;

            return View(exams);
        }


        // Step 3: hiển thị bài thi
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TakeExam(int examId)
        {
            var studentId = HttpContext.Session.GetInt32("studentId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            // Gọi Service để kiểm tra điều kiện thi
            var (exam, errorRedirect) = await _examService.CanTakeExamAsync(examId, studentId.Value);

            if (exam == null)
            {
                if (errorRedirect == "NotFound") return NotFound();

                // Xử lý lỗi từ Service
                var parts = errorRedirect.Split('|');
                TempData["Error"] = parts[0];
                int subjectId = int.Parse(parts[1]);
                return RedirectToAction("ChooseExam", new { subjectId });
            }

            // Lưu examId vào session 
            HttpContext.Session.SetInt32("currentExamId", examId);

            return View(exam);
        }

        // Step 4: nộp bài thi
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId, List<StudentAnswerModel> answers)
        {
            var studentId = HttpContext.Session.GetInt32("studentId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            // Gọi Service để thực hiện nộp bài và tính điểm
            var (submit, redirectAction) = await _examService.SubmitExamAsync(examId, studentId.Value, answers);

            if (submit == null) return NotFound();

            // Nếu đã nộp rồi (Service trả về redirect)
            if (redirectAction != null)
            {
                // redirectAction sẽ là "Result|submitId"
                var parts = redirectAction.Split('|');
                int submitId = int.Parse(parts[1]);
                return RedirectToAction("Result", new { submitId });
            }

            // Xóa session examId vì đã nộp xong
            HttpContext.Session.Remove("currentExamId");

            return RedirectToAction("Result", new { submitId = submit.SbId });
        }

        // Step 5: xem kết quả
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Result(int submitId)
        {
            var submit = await _examService.GetExamResultAsync(submitId);

            if (submit == null) return NotFound();

            ViewBag.ExamName = submit.Exam?.EName;
            return View(submit);
        }

        // API cho client gọi khi thoát trang
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> AutoSubmit()
        {
            var studentId = HttpContext.Session.GetInt32("studentId");
            var examId = HttpContext.Session.GetInt32("currentExamId");

            if (studentId == null || examId == null)
                return Json(new { success = false, message = "Không có phiên thi hiện tại" });

            // Gọi Service để tự động nộp
            var submit = await _examService.AutoSubmitAsync(studentId.Value, examId.Value);

            if (submit == null) // Đã nộp rồi
                return Json(new { success = false, message = "Đã nộp rồi" });

            // Xóa session examId vì đã nộp xong
            HttpContext.Session.Remove("currentExamId");

            return Json(new { success = true });
        }
    }
}
