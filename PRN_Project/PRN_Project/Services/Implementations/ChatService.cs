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

        public async Task<bool> SaveCommunityMessageAsync(int senderId, string content, int groupId)
        {
            // Kiểm tra nhóm có tồn tại không
            var group = await _repository.GetCommunityGroupAsync(groupId);
            if (group == null) return false;

            var message = new ChatMessage2
            {
                SenderId = senderId,
                Content = content,
                GroupId = groupId, // Gán GroupId
                ReceiverId = null, // Tin nhắn cộng đồng không có người nhận cá nhân
                Timestamp = DateTime.Now
            };

            await _repository.AddMessageAsync(message);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatMessage2>> LoadCommunityChatHistoryAsync(int groupId)
        {
            return await _repository.LoadCommunityChatHistoryAsync(groupId);
        }

        public async Task<ChatGroup?> CreateGroupWithMembersAsync(string groupName, int creatorId, List<string> memberEmails)
        {
            // 1. Tạo nhóm và thêm người tạo
            var group = await _repository.CreateGroupAsync(groupName, creatorId);

            // 2. Thêm các thành viên khác
            foreach (var email in memberEmails.Distinct())
            {
                var account = await _repository.GetAccountByEmailAsync(email);
                if (account != null && account.AId != creatorId)
                {
                    await _repository.AddMemberToGroupAsync(group.GroupId, account.AId);
                }
            }

            return group;
        }

        public async Task<ChatGroup?> GetGroupDetailsAsync(int groupId, int currentAccountId)
        {
            // Kiểm tra người dùng có phải là thành viên của nhóm không
            bool isMember = await _repository.IsMemberOfGroupAsync(groupId, currentAccountId);
            if (!isMember) return null;

            // Lấy chi tiết nhóm
            return await _repository.GetGroupByIdAsync(groupId);
        }

        public async Task<bool> SaveGroupMessageAsync(int senderId, string content, int groupId)
        {
            // Kiểm tra: Người gửi phải là thành viên của nhóm trước khi lưu
            if (!await _repository.IsMemberOfGroupAsync(groupId, senderId)) return false;

            var message = new ChatMessage2
            {
                SenderId = senderId,
                Content = content,
                GroupId = groupId,
                ReceiverId = null,
                Timestamp = DateTime.Now
            };

            await _repository.AddMessageAsync(message);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatMessage2>> LoadGroupChatHistoryAsync(int groupId)
        {
            return await _repository.LoadGroupChatHistoryAsync(groupId);
        }

        public async Task<List<ChatGroup>> GetUserGroupsAsync(int accountId)
        {
            return await _repository.GetGroupsByAccountIdAsync(accountId);
        }

        public async Task<bool> LeaveGroupAsync(int groupId, int accountId)
        {
            bool isMember = await _repository.IsMemberOfGroupAsync(groupId, accountId);
            if (!isMember) return true; // Hoặc false, tùy theo yêu cầu (người dùng đã không còn là thành viên)

            // *Tùy chọn*: Thêm logic kiểm tra người quản trị cuối cùng tại đây

            await _repository.RemoveMemberFromGroupAsync(groupId, accountId);
            return true;
        }
        public async Task<(bool Success, string Message)> AddMembersToExistingGroupAsync(int groupId, int currentAccountId, List<string> memberEmails)
        {
            // 1. Kiểm tra xem người yêu cầu có thuộc nhóm không (quyền admin nên được kiểm tra ở đây)
            bool isMember = await _repository.IsMemberOfGroupAsync(groupId, currentAccountId);
            if (!isMember)
            {
                return (false, "Bạn không có quyền thực hiện thao tác này.");
            }

            // 2. Lấy thông tin nhóm (Tùy chọn: kiểm tra nhóm có tồn tại không)
            var group = await _repository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return (false, "Nhóm chat không tồn tại.");
            }

            int addedCount = 0;

            // 3. Xử lý và thêm từng email
            foreach (var email in memberEmails.Distinct())
            {
                var account = await _repository.GetAccountByEmailAsync(email);

                if (account != null)
                {
                    // AddMemberToGroupAsync đã có logic kiểm tra trùng lặp
                    await _repository.AddMemberToGroupAsync(groupId, account.AId);
                    addedCount++;
                }
            }

            if (addedCount == 0 && memberEmails.Count > 0)
            {
                return (true, "Không tìm thấy người dùng hợp lệ nào để thêm vào nhóm.");
            }

            return (true, $"Đã thêm thành công {addedCount}/{memberEmails.Count} thành viên.");
        }
    }
}
