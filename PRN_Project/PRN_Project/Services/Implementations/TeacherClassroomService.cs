using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces; // Thêm
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels;
using PRN_Project.ViewModels.Submission;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PRN_Project.Services.Implementations
{
    public class TeacherClassroomService : ITeacherClassroomService
    {
        // Service phụ thuộc vào Repository
        private readonly ITeacherClassroomRepository _repo;

        public TeacherClassroomService(ITeacherClassroomRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Classroom>> GetClassroomsByTeacherAccountIdAsync(int accountId)
        {
            var teacher = await _repo.GetTeacherByAccountIdAsync(accountId);
            if (teacher == null)
            {
                return new List<Classroom>(); // Trả về ds rỗng nếu không tìm thấy teacher
            }
            return await _repo.GetClassroomsByTeacherIdAsync(teacher.TId);
        }

        public async Task<ClassroomDetailsDTO> GetClassroomDetailsAsync(int classroomId)
        {
            var classroom = await _repo.GetClassroomByIdAsync(classroomId);
            if (classroom == null) return null;

            var members = await _repo.GetMembersByClassroomIdAsync(classroomId);

            return new ClassroomDetailsDTO
            {
                ClassroomName = classroom.ClassName,
                Members = members
            };
        }

        public async Task<StudentSubmissionsDTO> GetStudentSubmissionsAsync(int studentId)
        {
            var student = await _repo.GetStudentByIdAsync(studentId);
            if (student == null) return null;

            var submissions = await _repo.GetSubmissionsByStudentIdAsync(studentId);

            return new StudentSubmissionsDTO
            {
                StudentName = student.SName,
                Submissions = submissions
            };
        }

        public async Task<Submit> GetSubmissionForEditAsync(int submitId)
        {
            return await _repo.GetSubmissionForEditAsync(submitId);
        }

        public async Task<Submit> UpdateSubmissionCommentAsync(int submitId, string comment)
        {
            var submission = await _repo.GetSubmissionByIdAsync(submitId);
            if (submission == null) return null;

            submission.Comment = comment;

            _repo.UpdateSubmission(submission); // Dùng Update (Sync)
            await _repo.SaveChangesAsync(); // Dùng Save (Async)

            return submission;
        }
        public async Task<SubmissionDetailViewModel> GetSubmissionDetailsAsync(int submitId)
        {
            var submission = await _repo.GetSubmissionWithDetailsAsync(submitId);
            if (submission == null || string.IsNullOrEmpty(submission.Exam.ExamContent))
            {
                return null;
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                // 1. Giải mã Câu hỏi (từ Exam)
                var examQuestions = JsonSerializer.Deserialize<List<ExamQuestionViewModel>>(
                    submission.Exam.ExamContent, jsonOptions);

                // 2. Giải mã Câu trả lời (từ Submit)
                var studentAnswers = JsonSerializer.Deserialize<List<StudentAnswerViewModel>>(
                    submission.Content, jsonOptions);

                // 3. Xử lý logic 0-based hay 1-based index
                bool isOneBasedIndex = studentAnswers.Any() && studentAnswers[0].QuestionIndex == 1;

                // 4. Tạo ViewModel
                var viewModel = new SubmissionDetailViewModel
                {
                    SubmitId = submission.SbId,
                    StudentName = submission.Student.SName,
                    ExamName = submission.Exam.EName,
                    Score = submission.Score,
                    TotalQuestions = examQuestions.Count,
                    CorrectCount = 0 // Sẽ đếm ở dưới
                };

                // 5. So sánh từng câu
                for (int i = 0; i < examQuestions.Count; i++)
                {
                    var question = examQuestions[i];

                    // Tìm câu trả lời của HS cho câu hỏi này
                    StudentAnswerViewModel studentAnswer = null;
                    if (isOneBasedIndex)
                        studentAnswer = studentAnswers.FirstOrDefault(a => a.QuestionIndex == (i + 1));
                    else
                        studentAnswer = studentAnswers.FirstOrDefault(a => a.QuestionIndex == i);

                    bool isCorrect = (studentAnswer != null &&
                                      studentAnswer.ChosenAnswer == question.CorrectAnswer);

                    if (isCorrect) viewModel.CorrectCount++;

                    viewModel.Questions.Add(new QuestionDetailViewModel
                    {
                        Index = i + 1,
                        QuestionText = question.Question,
                        Options = question.Options,
                        StudentAnswer = studentAnswer?.ChosenAnswer ?? " (Không trả lời)",
                        CorrectAnswer = question.CorrectAnswer,
                        IsCorrect = isCorrect
                    });
                }

                return viewModel;
            }
            catch (JsonException)
            {
                // Xử lý nếu JSON bị lỗi
                return null;
            }
        }
        }
}