using System.Text;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using PRN_Project.Models.RankingModels;

namespace PRN_Project.Services.Implementations
{
    public class RankingService : IRankingService
    {
        private readonly IRankingRepository _repo;

        public RankingService(IRankingRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Subject>> GetSubjectsForRankingAsync(int accountId, string role)
        {
            if (role == "Teacher")
                return await _repo.GetSubjectsForTeacherAsync(accountId);

            return await _repo.GetAllSubjectsAsync();
        }

        public async Task<List<SubjectExamRankingViewModel>> GetSubjectRankingAsync(int subjectId)
        {
            var exams = await _repo.GetExamsBySubjectAsync(subjectId);
            var result = new List<SubjectExamRankingViewModel>();

            foreach (var exam in exams)
            {
                var submits = await _repo.GetSubmitsByExamAsync(exam.EId);
                var rows = submits.Select((s, i) => new ExamRankingRow
                {
                    StudentId = s.SId,
                    StudentName = s.Student.SName,
                    Score = s.Score ?? 0,
                    RankPosition = i + 1
                }).ToList();

                result.Add(new SubjectExamRankingViewModel
                {
                    SubjectName = exam.Subject?.SuName ?? "Không rõ",
                    ExamId = exam.EId,
                    ExamName = exam.EName,
                    Rankings = rows
                });
            }

            return result;
        }

        public async Task<List<MyExamRankViewModel>> GetStudentRankingAsync(int accountId)
        {
            var student = await _repo.GetStudentByAccountIdAsync(accountId);
            if (student == null) return new List<MyExamRankViewModel>();

            var submits = await _repo.GetSubmitsByStudentIdAsync(student.SId);
            var result = new List<MyExamRankViewModel>();

            foreach (var s in submits)
            {
                var examSubmits = await _repo.GetSubmitsByExamAsync(s.EId);
                var allScores = examSubmits.Select(x => x.Score ?? 0).ToList();

                result.Add(new MyExamRankViewModel
                {
                    ExamName = s.Exam.EName,
                    SubjectName = s.Exam.Subject.SuName,
                    Score = s.Score ?? 0,
                    RankInExam = allScores.FindIndex(x => x == (s.Score ?? 0)) + 1,
                    TotalParticipants = allScores.Count
                });
            }

            return result;
        }

        public async Task<string> ExportRankingCsvAsync(int subjectId)
        {
            var exams = await _repo.GetExamsBySubjectAsync(subjectId);
            var sb = new StringBuilder();
            sb.AppendLine("Môn học,Bài thi,Học sinh,Điểm,Thứ hạng");

            foreach (var exam in exams)
            {
                var submits = await _repo.GetSubmitsByExamAsync(exam.EId);
                var rows = submits.Select((s, i) =>
                    $"{exam.Subject?.SuName},{exam.EName},{s.Student?.SName ?? "Unknown"},{(s.Score ?? 0):0.00},{i + 1}");

                sb.AppendLine(string.Join("\n", rows));
            }

            return sb.ToString();
        }
    }
}
