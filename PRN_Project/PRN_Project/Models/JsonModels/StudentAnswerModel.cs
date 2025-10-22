namespace PRN_Project.Models.JsonModels
{
    public class StudentAnswerModel
    {
        public int QuestionIndex { get; set; }      // Câu thứ mấy
        public string ChosenAnswer { get; set; } = string.Empty;
    }
}
