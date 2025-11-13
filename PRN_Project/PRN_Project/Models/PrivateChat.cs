using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN_Project.Models
{
    public class PrivateChat
    {
        // Khóa chính có thể là ID đơn
        [Key]
        public int ChatId { get; set; }

        // Đảm bảo ID nhỏ hơn luôn ở cột A để tránh trùng lặp
        public int UserAId { get; set; }
        public int UserBId { get; set; }

        public DateTime LastActive { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserAId")]
        public Account UserA { get; set; }

        [ForeignKey("UserBId")]
        public Account UserB { get; set; }
    }
}
