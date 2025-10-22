using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using PRN_Project.Models.JsonModels;

namespace PRN_Project.Models
{
    public class Answer
    {
        [Key]
        public int AwId { get; set; }

        [Required]
        public int EId { get; set; }
        public Exam? Exam { get; set; }

        public string? AnswerContent { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool? Status { get; set; }

        [NotMapped]
        public List<QuestionModel>? CorrectAnswers
        {
            get => string.IsNullOrEmpty(AnswerContent)
                ? new List<QuestionModel>()
                : JsonSerializer.Deserialize<List<QuestionModel>>(AnswerContent);
            set => AnswerContent = JsonSerializer.Serialize(value);
        }
    }
}
