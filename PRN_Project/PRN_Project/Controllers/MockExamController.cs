using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Models.JsonModels;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Controllers
{
    public class MockExamController : Controller
    {
        private readonly LmsDbContext _context;

        public MockExamController(LmsDbContext context)
        {
            _context = context;
        }

        // Step 1: chọn môn thi
        public IActionResult ChooseSubject()
        {
            var subjects = _context.Subjects.ToList();
            return View(subjects);
        }

        public IActionResult ChooseExam(int subjectId)
        {
            var sid = HttpContext.Session.GetInt32("studentId");
            if (sid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách exam của môn học
            var exams = _context.Exams.Where(e => e.SuId == subjectId).ToList();

            // Lấy danh sách exam mà học sinh đã làm
            var doneExamIds = _context.Submits
                .Where(r => r.SId == sid)
                .Select(r => r.EId)
                .ToList();

            // Truyền vào ViewBag
            ViewBag.Subject = _context.Subjects.FirstOrDefault(s => s.SuId == subjectId);
            ViewBag.DoneExams = doneExamIds;

            return View(exams);
        }


        // Step 3: hiển thị bài thi
        public IActionResult TakeExam(int examId)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null) return NotFound();

            var studentId = HttpContext.Session.GetInt32("studentId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            // ✅ Kiểm tra nếu đã nộp bài thì không được thi lại
            var submitted = _context.Submits.Any(s => s.SId == studentId && s.EId == examId);
            if (submitted)
            {
                TempData["Error"] = "Bạn đã nộp bài cho kỳ thi này, không thể làm lại.";
                return RedirectToAction("ChooseExam", new { subjectId = exam.SuId });
            }

            // ✅ Kiểm tra thời gian hợp lệ
            if (exam.StartTime > DateTime.Now || exam.EndTime < DateTime.Now)
            {
                TempData["Error"] = "Bài thi chưa đến giờ hoặc đã hết hạn.";
                return RedirectToAction("ChooseExam", new { subjectId = exam.SuId });
            }

            // Lưu examId vào session để nếu rời trang sẽ tự động nộp
            HttpContext.Session.SetInt32("currentExamId", examId);

            return View(exam);
        }

        // Step 4: nộp bài thi (gọi từ Submit hoặc tự động nộp)
        [HttpPost]
        public IActionResult SubmitExam(int examId, List<StudentAnswerModel> answers)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null) return NotFound();

            var studentId = HttpContext.Session.GetInt32("studentId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            // ✅ Nếu đã có submit, không cho nộp lại
            var existedSubmit = _context.Submits.FirstOrDefault(s => s.SId == studentId && s.EId == examId);
            if (existedSubmit != null)
            {
                return RedirectToAction("Result", new { submitId = existedSubmit.SbId });
            }

            // ✅ Tính điểm
            double score = 0;
            var questions = exam.Questions ?? new List<QuestionModel>();
            for (int i = 0; i < questions.Count; i++)
            {
                var studentAnswer = answers.FirstOrDefault(a => a.QuestionIndex == i);
                if (studentAnswer != null && studentAnswer.ChosenAnswer == questions[i].CorrectAnswer)
                {
                    score += 10.0 / questions.Count;
                }
            }

            var submit = new Submit
            {
                SId = studentId.Value,
                EId = examId,
                Score = Math.Round(score, 2),
                SubmitTime = DateTime.Now,
                StudentAnswers = answers
            };

            _context.Submits.Add(submit);
            _context.SaveChanges();

            // Xóa session examId vì đã nộp xong
            HttpContext.Session.Remove("currentExamId");

            return RedirectToAction("Result", new { submitId = submit.SbId });
        }

        // Step 5: xem kết quả
        public IActionResult Result(int submitId)
        {
            var submit = _context.Submits
                .Include(s => s.Exam)
                .Include(s => s.Student)
                .FirstOrDefault(s => s.SbId == submitId);

            if (submit == null) return NotFound();

            ViewBag.ExamName = submit.Exam?.EName;
            return View(submit);
        }

        // ✅ API cho client gọi khi thoát trang
        [HttpPost]
        public IActionResult AutoSubmit()
        {
            var studentId = HttpContext.Session.GetInt32("studentId");
            var examId = HttpContext.Session.GetInt32("currentExamId");

            if (studentId == null || examId == null)
                return Json(new { success = false, message = "Không có phiên thi hiện tại" });

            // Nếu đã nộp rồi thì bỏ qua
            if (_context.Submits.Any(s => s.SId == studentId && s.EId == examId))
                return Json(new { success = false, message = "Đã nộp rồi" });

            // Nộp bài trắng (không có câu trả lời)
            var submit = new Submit
            {
                SId = studentId.Value,
                EId = examId.Value,
                Score = 0,
                SubmitTime = DateTime.Now,
                StudentAnswers = new List<StudentAnswerModel>()
            };

            _context.Submits.Add(submit);
            _context.SaveChanges();

            HttpContext.Session.Remove("currentExamId");

            return Json(new { success = true });
        }
    }
}
