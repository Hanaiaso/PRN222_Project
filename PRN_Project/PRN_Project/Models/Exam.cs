using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using PRN_Project.Models.JsonModels;

namespace PRN_Project.Models
{
    public class Exam
    {
        [Key]
        public int EId { get; set; }

        public int? SuId { get; set; }           // optional if you want exam without subject
        public Subject? Subject { get; set; }

        [Required, MaxLength(100)]
        public string EName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string? ExamContent { get; set; }
        public bool Status { get; set; } = true;

        [NotMapped]
        public List<QuestionModel>? Questions
        {
            get => string.IsNullOrEmpty(ExamContent)
                ? new List<QuestionModel>()
                : JsonSerializer.Deserialize<List<QuestionModel>>(ExamContent);
            set => ExamContent = JsonSerializer.Serialize(value);
        }

        // Navigation
        public ICollection<Answer>? Answers { get; set; }
        public ICollection<Submit>? Submits { get; set; }
        public ICollection<Rank>? Ranks { get; set; }
    }
}
