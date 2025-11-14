using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;

namespace PRN_Project.Models
{
    public class Classroom
    {
        [Key]
        public int ClassroomId { get; set; }
        [Required]
        public int Tid { get; set; }           // FK -> Teacher
        public string ClassName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string ClassDescription { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; } = DateTime.Now;

        // Navigation
        public Teacher? Teacher { get; set; }
        public virtual ICollection<Post>? Posts { get; set; } 
        public virtual ICollection<ClassroomMember>? Members { get; set; } 
    }
}
