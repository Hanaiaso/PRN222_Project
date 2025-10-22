using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using PRN_Project.Models.JsonModels;

namespace PRN_Project.Models
{
    public class Submit
    {
        [Key]
        public int SbId { get; set; }

        [Required]
        public int SId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public int EId { get; set; }
        public Exam? Exam { get; set; }

        public double? Score { get; set; }     // or decimal

        public string? Content { get; set; }   // aggregated content if needed

        public DateTime SubmitTime { get; set; }

        public string? Comment { get; set; }

        [NotMapped]
        public List<StudentAnswerModel>? StudentAnswers
        {
            get => string.IsNullOrEmpty(Content)
                ? new List<StudentAnswerModel>()
                : JsonSerializer.Deserialize<List<StudentAnswerModel>>(Content);
            set => Content = JsonSerializer.Serialize(value);
        }
    }
}
