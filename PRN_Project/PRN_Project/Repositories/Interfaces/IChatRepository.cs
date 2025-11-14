using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<Account?> GetAccountByEmailAsync(string email);

        // Cập nhật phương thức: Không cần PrivateChat, chỉ cần lưu tin nhắn
        Task<List<ChatMessage2>> GetChatHistoryAsync(int user1Id, int user2Id); // MỚI
        Task AddMessageAsync(ChatMessage2 message);
        Task SaveChangesAsync();
        Task<ChatGroup?> GetCommunityGroupAsync(int groupId); // MỚI: Lấy nhóm cộng đồng
        Task<List<ChatMessage2>> LoadCommunityChatHistoryAsync(int groupId); // MỚI: Lịch sử chat cộng đồng
        // Thêm các phương thức mới cho Chat Nhóm
        Task<ChatGroup> CreateGroupAsync(string groupName, int creatorId);
        Task AddMemberToGroupAsync(int groupId, int accountId, bool isAdmin = false);
        Task<bool> IsMemberOfGroupAsync(int groupId, int accountId);
        Task<ChatGroup?> GetGroupByIdAsync(int groupId);
        Task<List<ChatMessage2>> LoadGroupChatHistoryAsync(int groupId);
        Task<List<ChatGroup>> GetGroupsByAccountIdAsync(int accountId);
    }
}
