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
        Task<bool> SaveCommunityMessageAsync(int senderId, string content, int groupId); // MỚI
        Task<List<ChatMessage2>> LoadCommunityChatHistoryAsync(int groupId); // MỚI
        
        // Thêm các phương thức mới cho Chat Nhóm
        Task<ChatGroup?> CreateGroupWithMembersAsync(string groupName, int creatorId, List<string> memberEmails);
        Task<ChatGroup?> GetGroupDetailsAsync(int groupId, int currentAccountId);
        Task<bool> SaveGroupMessageAsync(int senderId, string content, int groupId);
        Task<List<ChatMessage2>> LoadGroupChatHistoryAsync(int groupId);
        Task<List<ChatGroup>> GetUserGroupsAsync(int accountId);
        Task<bool> LeaveGroupAsync(int groupId, int accountId);
        Task<(bool Success, string Message)> AddMembersToExistingGroupAsync(int groupId, int currentAccountId, List<string> memberEmails);
    }
}

