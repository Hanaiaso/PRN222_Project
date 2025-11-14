using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly LmsDbContext _context; // dùng tạm cho SelectList

        public ExamController(IExamService examService, LmsDbContext context)
        {
            _examService = examService;
            _context = context;
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index()
        {
            var exams = await _examService.GetAllExamsAsync();
            return View(exams);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var exam = await _examService.GetExamByIdAsync(id.Value);
            if (exam == null) return NotFound();
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName");
            return View();
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                await _examService.CreateExamAsync(exam);
                TempData["Success"] = "Tạo bài thi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _examService.GetExamByIdAsync(id.Value);
            if (exam == null) return NotFound();

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam, string questionData)
        {
            if (id != exam.EId) return NotFound();
            if (ModelState.IsValid)
            {
                var dbExam = await _examService.GetExamByIdAsync(id);
                if (dbExam == null) return NotFound();

                dbExam.EName = exam.EName;
                dbExam.SuId = exam.SuId;
                dbExam.StartTime = exam.StartTime;
                dbExam.EndTime = exam.EndTime;
                dbExam.Status = exam.Status;

                if (!string.IsNullOrEmpty(questionData))
                    dbExam.ExamContent = questionData;

                await _examService.UpdateExamAsync(dbExam);
                TempData["Success"] = "Cập nhật bài thi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _examService.GetExamByIdAsync(id.Value);
            if (exam == null) return NotFound();
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam != null)
            {
                await _examService.DeleteExamAsync(exam);
                TempData["Success"] = "Xóa bài thi thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
