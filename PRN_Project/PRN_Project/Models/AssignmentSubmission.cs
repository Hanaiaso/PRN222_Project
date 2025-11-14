using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class AssignmentSubmission
    {
        [Key]
        public int SubmissionId { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public int Sid { get; set; }                  // Student ID
        public string Content { get; set; } = string.Empty;
        public DateTime SubmitTime { get; set; } = DateTime.Now;
        public string Status { get; set; } = string.Empty;         // Submitted / Graded
        public double? Grade { get; set; }

        // Navigation
        public Post? Post { get; set; }
        public Student? Student { get; set; }
    }
}
