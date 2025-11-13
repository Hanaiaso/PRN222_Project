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
    }
}
