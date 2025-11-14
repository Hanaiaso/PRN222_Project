using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN_Project.Models
{
    public class ChatGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        // Loại: 1=Private Group (nhóm nhỏ), 2=Community (cộng đồng/lớp học)
        public int Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<GroupMember> Members { get; set; }
        public ICollection<ChatMessage2> Messages { get; set; }
    }
}
