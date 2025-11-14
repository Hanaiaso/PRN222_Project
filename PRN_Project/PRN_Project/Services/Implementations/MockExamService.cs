using PRN_Project.Models;
using PRN_Project.Models.JsonModels;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class MockExamService : IMockExamService
    {
        private readonly IMockExamRepository _repository;

        public MockExamService(IMockExamRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            return await _repository.GetAllSubjectsAsync();
        }

        public async Task<(List<Exam> Exams, Subject? Subject, List<int> DoneExamIds)> GetExamsForStudentAsync(int subjectId, int studentId)
        {
            var exams = await _repository.GetExamsBySubjectIdAsync(subjectId);
            var subject = await _repository.GetExamByIdAsync(subjectId) is Exam ex ? ex.Subject : null; // Giả định Subject có thể lấy qua Exam hoặc dùng Repository

            // Do Subject không có Repository riêng, ta dùng cách tìm Subject đơn giản hơn:
            var subjects = await _repository.GetAllSubjectsAsync();
            var targetSubject = subjects.FirstOrDefault(s => s.SuId == subjectId);

            var doneExamIds = await _repository.GetDoneExamIdsByStudentIdAsync(studentId);

            return (exams, targetSubject, doneExamIds);
        }

        // Phương thức kiểm tra điều kiện thi
        public async Task<(Exam? Exam, string? ErrorRedirectAction)> CanTakeExamAsync(int examId, int studentId)
        {
            var exam = await _repository.GetExamByIdAsync(examId);
            if (exam == null) return (null, "NotFound");

            // Kiểm tra đã nộp bài
            var submitted = await _repository.IsExamSubmittedAsync(studentId, examId);
            if (submitted)
            {
                return (null, $"Bạn đã nộp bài cho kỳ thi này, không thể làm lại.|{exam.SuId}");
            }

            // Kiểm tra thời gian hợp lệ
            if (exam.StartTime > DateTime.Now || exam.EndTime < DateTime.Now)
            {
                return (null, $"Bài thi chưa đến giờ hoặc đã hết hạn.|{exam.SuId}");
            }

            return (exam, null);
        }

        // Phương thức nộp bài chính thức
        public async Task<(Submit? Submit, string? RedirectAction)> SubmitExamAsync(int examId, int studentId, List<StudentAnswerModel> answers)
        {
            var exam = await _repository.GetExamByIdAsync(examId);
            if (exam == null) return (null, "NotFound");

            // Kiểm tra nếu đã có submit, không cho nộp lại
            var existedSubmit = await _repository.GetSubmitByStudentAndExamIdAsync(studentId, examId);
            if (existedSubmit != null)
            {
                return (existedSubmit, $"Result|{existedSubmit.SbId}");
            }

            // 1. Logic Tính điểm
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

            // 2. Tạo đối tượng Submit
            var submit = new Submit
            {
                SId = studentId,
                EId = examId,
                Score = Math.Round(score, 2),
                SubmitTime = DateTime.Now,
                StudentAnswers = answers
            };

            // 3. Lưu vào DB
            await _repository.AddSubmitAsync(submit);
            await _repository.SaveChangesAsync();

            return (submit, null);
        }

        // Phương thức tự động nộp bài (bài trắng)
        public async Task<Submit?> AutoSubmitAsync(int studentId, int examId)
        {
            // Nếu đã nộp rồi thì bỏ qua
            if (await _repository.IsExamSubmittedAsync(studentId, examId))
            {
                return null;
            }

            // Nộp bài trắng (không có câu trả lời)
            var submit = new Submit
            {
                SId = studentId,
                EId = examId,
                Score = 0,
                SubmitTime = DateTime.Now,
                StudentAnswers = new List<StudentAnswerModel>()
            };

            await _repository.AddSubmitAsync(submit);
            await _repository.SaveChangesAsync();

            return submit;
        }

        public async Task<Submit?> GetExamResultAsync(int submitId)
        {
            return await _repository.GetSubmitWithDetailsAsync(submitId);
        }
    }
}
