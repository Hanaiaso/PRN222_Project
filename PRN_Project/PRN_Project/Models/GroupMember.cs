using System.ComponentModel.DataAnnotations.Schema;

namespace PRN_Project.Models
{
    public class GroupMember
    {
        // Khóa chính kép hoặc khóa chính đơn (tùy chọn)
        public int GroupId { get; set; }
        public int AccountId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public bool IsAdmin { get; set; } = false;

        // Navigation properties
        [ForeignKey("GroupId")]
        public ChatGroup Group { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; } // Giả sử Model Account đã tồn tại
    }
}
