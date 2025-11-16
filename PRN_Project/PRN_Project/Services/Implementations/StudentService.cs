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
            if (student == null) return null;

            var rawSubmissions = await _studentRepo.GetSubmissionsWithDetailsAsync(studentId);

            // 1. Tạo danh sách phẳng (flat list) các bài nộp
            var submissions = rawSubmissions.Select(s =>
            {
                // Logic tính toán số câu hỏi và câu đúng
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
                                int questionIndex = studentAnswer.QuestionIndex ;
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
                    TeacherComment = s.Comment
                };
            }).ToList();

            // 2. Nhóm danh sách phẳng theo Môn học (cho Bảng và Biểu đồ cột)
            var groupedProgress = submissions
                .GroupBy(s => s.SubjectName)
                .Select(g => new SubjectProgressViewModel
                {
                    SubjectName = g.Key,
                    Submissions = g.OrderBy(s => s.SubmitTime).ToList(),
                    SubjectExamsTaken = g.Count(),
                    SubjectAverageScore = g.Average(s => s.Score)
                })
                .OrderBy(sp => sp.SubjectName)
                .ToList();

            // 3. Tạo dữ liệu cho Biểu đồ đường (Line Chart) có thể lọc
            var lineChartDatasets = new Dictionary<string, object>();
            
            // 3b. Thêm từng môn học (đã được sắp xếp)
            foreach (var subject in groupedProgress)
            {
                lineChartDatasets[subject.SubjectName] = new
                {
                    labels = subject.Submissions.Select(s => $"{s.ExamName} ({s.SubmitTime:dd/MM})"),
                    data = subject.Submissions.Select(s => s.Score)
                };
            }

            // 4. Lắp ráp ViewModel chính
            var viewModel = new StudentProgressViewModel
            {
                StudentId = student.SId,
                StudentName = student.SName,
                SubjectProgressList = groupedProgress,
                TotalExamsTaken = submissions.Count,
                AverageScore = submissions.Any() ? submissions.Average(s => s.Score) : (double?)null,

                // Dữ liệu biểu đồ cột
                BarChartLabels = JsonSerializer.Serialize(groupedProgress.Select(g => g.SubjectName)),
                BarChartData = JsonSerializer.Serialize(groupedProgress.Select(g => g.SubjectAverageScore)),

                // Dữ liệu biểu đồ đường
                LineChartDataJson = JsonSerializer.Serialize(lineChartDatasets)
            };

            return viewModel;
        }
    }
}