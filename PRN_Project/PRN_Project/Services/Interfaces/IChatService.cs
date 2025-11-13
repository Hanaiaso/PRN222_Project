using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    // DTO để dễ dàng truyền dữ liệu User
    public class UserSessionInfo
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = "Anonymous";
    }
    public interface IChatService
    {
        // Kiểm tra Session và trả về thông tin người dùng
        Task<UserSessionInfo?> GetUserSessionInfoAsync(int? accountId, string? userEmail);

        // Cập nhật: Chỉ trả về thông tin người dùng mục tiêu
        Task<Account?> FindTargetUserAsync(string targetEmail, int currentAccountId); // MỚI

        // Cập nhật: Lưu tin nhắn 1-1
        Task SavePrivateMessageAsync(int senderId, string content, int receiverId); // MỚI

        // Cập nhật: Tải lịch sử chat 1-1
        Task<List<ChatMessage2>> LoadPrivateChatHistoryAsync(int user1Id, int user2Id); // MỚI
    }
}
