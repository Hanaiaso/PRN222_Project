using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class ClassroomMember
    {
        [Key]
        public int MemberId { get; set; }
        [Required]
        public int ClassroomId { get; set; }
        [Required]
        public int Sid { get; set; }              // FK -> Student
        public DateTime JoinDate { get; set; } = DateTime.Now;

        // Navigation
        public Classroom? Classroom { get; set; }
        public Student? Student { get; set; }
    }
}
