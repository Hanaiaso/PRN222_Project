using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;

        public ChatService(IChatRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserSessionInfo?> GetUserSessionInfoAsync(int? accountId, string? userEmail)
        {
            if (accountId == null)
            {
                return null; // Không có session, không hợp lệ
            }

            // Lấy tên người dùng từ DB (FullName) nếu có, nếu không dùng Email
            var account = await _repository.GetAccountByIdAsync(accountId.Value);
            string userName = account?.Email ?? userEmail ?? $"User_{accountId.Value}";

            return new UserSessionInfo
            {
                AccountId = accountId.Value,
                UserName = userName
            };
        }

        public async Task<Account?> FindTargetUserAsync(string targetEmail, int currentAccountId)
        {
            var targetUser = await _repository.GetAccountByEmailAsync(targetEmail);

            // Kiểm tra: Phải tồn tại và không chat với chính mình
            if (targetUser == null || targetUser.AId == currentAccountId)
            {
                return null;
            }

            return targetUser;
        }

        public async Task SavePrivateMessageAsync(int senderId, string content, int receiverId)
        {
            var message = new ChatMessage2
            {
                SenderId = senderId,
                Content = content,
                ReceiverId = receiverId, // Gán ReceiverId
                GroupId = null, // Đảm bảo là chat cá nhân
                Timestamp = DateTime.Now
            };

            await _repository.AddMessageAsync(message);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<ChatMessage2>> LoadPrivateChatHistoryAsync(int user1Id, int user2Id)
        {
            return await _repository.GetChatHistoryAsync(user1Id, user2Id);
        }
    }
}
