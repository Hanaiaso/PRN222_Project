using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN_Project.Models
{
    public class ChatMessage2
    {
        [Key]
        public long MessageId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // FK tới người gửi (Account)
        public int SenderId { get; set; }

        // FK tới nhóm (nếu là tin nhắn nhóm/cộng đồng). Null nếu là tin nhắn cá nhân.
        public int? GroupId { get; set; }

        // FK tới người nhận (nếu là tin nhắn cá nhân)
        public int? ReceiverId { get; set; }

        // Navigation properties
        [ForeignKey("SenderId")]
        public Account Sender { get; set; }

        [ForeignKey("GroupId")]
        public ChatGroup? Group { get; set; }

        [ForeignKey("ReceiverId")]
        public Account? Receiver { get; set; }
    }
}
