using System.Text.Json.Serialization;

namespace PRN_Project.ViewModels
{
    public class StudentAnswerViewModel
    {
        // Phải khớp với key trong JSON của Submit.Content
        [JsonPropertyName("QuestionIndex")]
        public int QuestionIndex { get; set; }

        [JsonPropertyName("ChosenAnswer")]
        public string ChosenAnswer { get; set; }
    }
}