using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Student
    {
        [Key]
        public int SId { get; set; }

        [Required]
        public int AId { get; set; }                     
        public Account? Account { get; set; }

        [Required, MaxLength(100)]
        public string SName { get; set; } = null!;

        [MaxLength(20)]
        public string? Gender { get; set; }

        public DateTime? Dob { get; set; }

        // Navigation
        public ICollection<Answer>? Answers { get; set; }
        public ICollection<Submit>? Submits { get; set; }
        public ICollection<ExamRank>? ExamRanks { get; set; }
    }
}
