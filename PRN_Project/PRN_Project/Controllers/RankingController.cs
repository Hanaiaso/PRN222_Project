using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using PRN_Project.Models.RankingModels;
using PRN_Project.Services.Interfaces;
using System.Security.Claims;
using System.Text;

namespace PRN_Project.Controllers
{
    [Authorize]
    public class RankingController : Controller
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> SubjectRanking(int? subjectId)
        {
            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            var subjects = await _rankingService.GetSubjectsForRankingAsync(accountId, role);

            ViewBag.Subjects = new SelectList(subjects, "SuId", "SuName", subjectId);

            if (subjectId == null)
                return View(new List<SubjectExamRankingViewModel>());

            var result = await _rankingService.GetSubjectRankingAsync(subjectId.Value);
            return View(result);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyRanking()
        {
            var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _rankingService.GetStudentRankingAsync(accountId);
            return View(result);
        }

        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> ExportCsv(int subjectId)
        {
            var csvData = await _rankingService.ExportRankingCsvAsync(subjectId);
            var fileName = $"Ranking_Subject_{subjectId}.csv";
            return File(Encoding.UTF8.GetBytes(csvData), "text/csv", fileName);
        }
    }
}
