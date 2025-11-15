using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces; // Thêm
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels; // Thêm
using System.Text.Json; // Thêm

namespace PRN_Project.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly JsonSerializerOptions _jsonOptions;

        public StudentService(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // Action 1: Dashboard
        public async Task<Student> GetStudentByAccountIdAsync(int accountId)
        {
            return await _studentRepo.GetStudentByAccountIdAsync(accountId);
        }

        // Action 2: StudentProgress
        public async Task<StudentProgressViewModel> GetStudentProgressReportAsync(int studentId)
        {
            var student = await _studentRepo.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                return null; // Hoặc ném một Exception
            }

            var rawSubmissions = await _studentRepo.GetSubmissionsWithDetailsAsync(studentId);

            // === TOÀN BỘ LOGIC TÍNH TOÁN CỦA BẠN ĐƯỢC CHUYỂN VÀO ĐÂY ===
            var submissions = rawSubmissions.Select(s =>
            {
                int questionCount = 0;
                List<ExamQuestionViewModel> examQuestions = null;
                if (!string.IsNullOrEmpty(s.Exam.ExamContent))
                {
                    try
                    {
                        examQuestions = JsonSerializer.Deserialize<List<ExamQuestionViewModel>>(
                            s.Exam.ExamContent, _jsonOptions);
                        questionCount = examQuestions?.Count ?? 0;
                    }
                    catch (JsonException) { questionCount = 0; }
                }

                int correctCount = 0;
                if (examQuestions != null && !string.IsNullOrEmpty(s.Content))
                {
                    try
                    {
                        var studentAnswers = JsonSerializer.Deserialize<List<StudentAnswerViewModel>>(
                            s.Content, _jsonOptions);

                        if (studentAnswers != null)
                        {
                            foreach (var studentAnswer in studentAnswers)
                            {
                                int questionIndex = studentAnswer.QuestionIndex - 1;
                                if (questionIndex >= 0 && questionIndex < examQuestions.Count)
                                {
                                    if (studentAnswer.ChosenAnswer == examQuestions[questionIndex].CorrectAnswer)
                                    {
                                        correctCount++;
                                    }
                                }
                            }
                        }
                    }
                    catch (JsonException) { /* Bỏ qua */ }
                }

                TimeSpan? examDuration = s.Exam.EndTime - s.Exam.StartTime;

                return new StudentSubmissionViewModel
                {
                    ExamName = s.Exam.EName,
                    SubjectName = s.Exam.Subject.SuName,
                    Score = s.Score,
                    SubmitTime = s.SubmitTime,
                    NumberOfQuestions = questionCount,
                    CorrectAnswers = correctCount,
                    ExamDuration = examDuration,
                    TeacherComment = s.Comment // <-- Đừng quên thêm cả comment
                };
            }).ToList();
            // === KẾT THÚC LOGIC ===

            // Lắp ráp ViewModel cuối cùng
            var viewModel = new StudentProgressViewModel
            {
                StudentId = student.SId,
                StudentName = student.SName,
                Submissions = submissions,
                TotalExamsTaken = submissions.Count,
                AverageScore = submissions.Any() ? submissions.Average(s => s.Score) : (double?)null
            };

            viewModel.ChartLabels = JsonSerializer.Serialize(submissions.Select(s => $"{s.ExamName} ({s.SubmitTime:dd/MM})"));
            viewModel.ChartData = JsonSerializer.Serialize(submissions.Select(s => s.Score));

            return viewModel;
        }
    }
}