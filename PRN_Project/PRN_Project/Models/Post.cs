using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public int ClassroomId { get; set; }
        [Required]
        public int Aid { get; set; }                  // FK -> Account
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string PostType { get; set; } = string.Empty;          // Announcement / Assignment
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }

        // Navigation
        public Classroom? Classroom { get; set; }
        public Account? Account { get; set; }
        public ICollection<AssignmentSubmission>? Submissions { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
