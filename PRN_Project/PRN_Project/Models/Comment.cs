using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Hosting;

namespace PRN_Project.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int Aid { get; set; }
        public string Content { get; set; } = null!;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        // Navigation properties
        public Post? Post { get; set; }
        public Account? Account { get; set; }
    }
}
