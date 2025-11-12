using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class RankingController : Controller
    {
        private readonly LmsDbContext _context;

        public RankingController(LmsDbContext context)
        {
            _context = context;
        }

        // 👨‍🏫 GIÁO VIÊN XEM XẾP HẠNG THEO MÔN HỌC
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> SubjectRanking(int? subjectId)
        {
            ViewBag.Subjects = new SelectList(await _context.Subjects.ToListAsync(), "SuId", "SuName", subjectId);

            if (subjectId == null)
                return View(new List<SubjectExamRankingViewModel>());

            var exams = await _context.Exams
                .Include(e => e.Subject)
                .Where(e => e.SuId == subjectId)
                .ToListAsync();

            var result = new List<SubjectExamRankingViewModel>();

            foreach (var exam in exams)
            {
                var ranking = await _context.Submits
                    .Include(s => s.Student)
                    .Where(s => s.EId == exam.EId)
                    .OrderByDescending(s => s.Score)
                    .Select(s => new ExamRankingRow
                    {
                        StudentId = s.SId,
                        StudentName = s.Student.SName,
                        Score = s.Score ?? 0
                    })
                    .ToListAsync();

                int pos = 1;
                foreach (var r in ranking)
                    r.RankPosition = pos++;

                result.Add(new SubjectExamRankingViewModel
                {
                    SubjectName = exam.Subject?.SuName ?? "Không rõ",
                    ExamId = exam.EId,
                    ExamName = exam.EName,
                    Rankings = ranking
                });
            }

            return View(result);
        }


        // 👩‍🎓 HỌC SINH XEM XẾP HẠNG CỦA MÌNH TRONG CÁC BÀI THI
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyRanking()
        {
            // Lấy ID tài khoản đăng nhập hiện tại
            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.AId == accountId);

            if (student == null)
                return NotFound("Không tìm thấy thông tin học sinh.");

            // Lấy tất cả các bài thi mà học sinh này đã nộp
            var myRanks = await _context.Submits
                .Include(s => s.Exam)
                .Where(s => s.SId == student.SId)
                .Select(s => new MyExamRankViewModel
                {
                    ExamName = s.Exam.EName,
                    SubjectName = s.Exam.Subject.SuName,
                    Score = s.Score ?? 0,
                    RankInExam = 0 // sẽ tính ở dưới
                })
                .ToListAsync();

            // Tính thứ hạng trong từng bài thi
            foreach (var item in myRanks)
            {
                var exam = await _context.Exams.FirstOrDefaultAsync(e => e.EName == item.ExamName);
                if (exam == null) continue;

                var allScores = await _context.Submits
                    .Where(s => s.EId == exam.EId)
                    .OrderByDescending(s => s.Score)
                    .Select(s => s.Score ?? 0)
                    .ToListAsync();

                var myScore = item.Score;
                item.RankInExam = allScores.FindIndex(s => s == myScore) + 1;
                item.TotalParticipants = allScores.Count;
            }

            return View(myRanks);
        }

        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> ExportCsv(int subjectId)
        {
            // Lấy dữ liệu giống như SubjectRanking
            var examsInSubject = await _context.Exams
                .Where(e => e.SuId == subjectId)
                .Include(e => e.Submits)
                    .ThenInclude(s => s.Student)
                .ToListAsync();

            if (!examsInSubject.Any())
                return NotFound("Không tìm thấy dữ liệu cho môn học này.");

            var csv = new StringBuilder();
            csv.AppendLine("Môn học,Bài thi,Học sinh,Điểm,Thứ hạng");

            foreach (var exam in examsInSubject)
            {
                var rankings = exam.Submits
                    .OrderByDescending(s => s.Score ?? 0)
                    .Select((s, index) => new
                    {
                        StudentName = s.Student?.SName ?? "Unknown",
                        Score = s.Score ?? 0,
                        Rank = index + 1
                    }).ToList();

                foreach (var r in rankings)
                {
                    csv.AppendLine($"{exam.Subject?.SuName},{exam.EName},{r.StudentName},{r.Score.ToString("0.00")},{r.Rank}");
                }
            }

            var fileName = $"Ranking_Subject_{subjectId}.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }
    }


    // 🔹 ViewModels
    public class SubjectRankingViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public double AverageScore { get; set; }
        public int RankPosition { get; set; }
    }

    public class MyExamRankViewModel
    {
        public string ExamName { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public double Score { get; set; }
        public int RankInExam { get; set; }
        public int TotalParticipants { get; set; }
    }
    public class SubjectExamRankingViewModel
    {
        public string SubjectName { get; set; } = null!;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = null!;
        public List<ExamRankingRow> Rankings { get; set; } = new();
    }

    public class ExamRankingRow
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public double Score { get; set; }
        public int RankPosition { get; set; }
    }
}
