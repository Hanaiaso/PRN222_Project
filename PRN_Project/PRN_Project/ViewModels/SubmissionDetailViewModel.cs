using System.Collections.Generic;

namespace PRN_Project.ViewModels.Submission
{
    // ViewModel chính cho trang "Chi tiết bài nộp"
    public class SubmissionDetailViewModel
    {
        public int SubmitId { get; set; }
        public string StudentName { get; set; }
        public string ExamName { get; set; }
        public double? Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }

        // Danh sách các câu hỏi chi tiết
        public List<QuestionDetailViewModel> Questions { get; set; } = new List<QuestionDetailViewModel>();
    }
}