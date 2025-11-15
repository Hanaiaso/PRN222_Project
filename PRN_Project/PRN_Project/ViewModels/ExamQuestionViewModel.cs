using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PRN_Project.ViewModels
{
    
    public class ExamQuestionViewModel
    {
        // Tên thuộc tính phải khớp với key trong JSON
        // Dùng JsonPropertyName để đảm bảo an toàn
        [JsonPropertyName("Question")]
        public string Question { get; set; }

        [JsonPropertyName("Options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("CorrectAnswer")]
        public string CorrectAnswer { get; set; }
    }
}
