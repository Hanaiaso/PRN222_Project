using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Models.JsonModels;

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

        // Step 2: chọn bài thi theo môn
        public IActionResult ChooseExam(int subjectId)
        {
            var exams = _context.Exams
                .Where(e => e.SuId == subjectId && e.Status)
                .ToList();
            ViewBag.Subject = _context.Subjects.Find(subjectId);
            return View(exams);
        }

        // Step 3: hiển thị bài thi
        public IActionResult TakeExam(int examId)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null) return NotFound();

            // kiểm tra thời gian hợp lệ
            if (exam.StartTime > DateTime.Now || exam.EndTime < DateTime.Now)
            {
                TempData["Error"] = "Bài thi chưa đến giờ hoặc đã hết hạn.";
                return RedirectToAction("ChooseExam", new { subjectId = exam.SuId });
            }

            return View(exam);
        }

        // Step 4: nộp bài thi
        [HttpPost]
        public IActionResult SubmitExam(int examId, List<StudentAnswerModel> answers)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null) return NotFound();

            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            // Tính điểm
            double score = 0;
            var questions = exam.Questions!;
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

            return RedirectToAction("Result", new { submitId = submit.SbId });
        }

        // Step 5: xem kết quả
        public IActionResult Result(int submitId)
        {
            var submit = _context.Submits
                .Where(s => s.SbId == submitId)
                .FirstOrDefault();

            if (submit == null) return NotFound();

            var exam = _context.Exams.Find(submit.EId);
            ViewBag.ExamName = exam?.EName;

            return View(submit);
        }
    }
}

