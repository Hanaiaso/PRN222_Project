namespace PRN_Project.ViewModels.Submission
{
    // Đại diện cho 1 câu hỏi đã được so sánh
    public class QuestionDetailViewModel
    {
        public int Index { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}