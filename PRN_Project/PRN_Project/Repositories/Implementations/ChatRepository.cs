using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Repositories.Implementations
{
    public class ChatRepository : IChatRepository
    {
        private readonly LmsDbContext _context;

        public ChatRepository(LmsDbContext context)
        {
            _context = context;
        }
        // Triển khai GetAccountByEmailAsync
        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            // Sử dụng DbContext để tìm Account dựa trên Email
            // Note: Đảm bảo Email trong DB không phân biệt chữ hoa/thường hoặc sử dụng ToLower()
            return await _context.Accounts
                                 .FirstOrDefaultAsync(a => a.Email == email);
        }

        // Triển khai SaveChangesAsync
        public async Task SaveChangesAsync()
        {
            // Gọi phương thức SaveChangesAsync của DbContext để lưu các thay đổi vào DB
            await _context.SaveChangesAsync();
        }

        // --- Các phương thức khác (giữ nguyên hoặc đã được triển khai) ---

        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts.FindAsync(accountId);
        }

        public async Task<List<ChatMessage2>> GetChatHistoryAsync(int user1Id, int user2Id)
        {
            // Logic truy vấn lịch sử chat 1-1
            return await _context.ChatMessages2
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id && m.GroupId == null) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id && m.GroupId == null))
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task AddMessageAsync(ChatMessage2 message)
        {
            await _context.ChatMessages2.AddAsync(message);
        }

        public async Task<ChatGroup?> GetCommunityGroupAsync(int groupId)
        {
            // Giả định Community Group có GroupId là 1
            return await _context.ChatGroups.FindAsync(groupId);
        }

        public async Task<List<ChatMessage2>> LoadCommunityChatHistoryAsync(int groupId)
        {
            // Truy vấn lịch sử tin nhắn chỉ dành cho GroupId này
            return await _context.ChatMessages2
                .Where(m => m.GroupId == groupId)
                .Include(m => m.Sender)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<ChatGroup> CreateGroupAsync(string groupName, int creatorId)
        {
            var newGroup = new ChatGroup
            {
                Name = groupName,
                Type = 1, // Giả định Type=1 là Private Group
                CreatedAt = DateTime.Now,
            };
            await _context.ChatGroups.AddAsync(newGroup);
            await _context.SaveChangesAsync();

            // Thêm người tạo nhóm làm thành viên quản trị
            await AddMemberToGroupAsync(newGroup.GroupId, creatorId, true);
            return newGroup;
        }

        public async Task AddMemberToGroupAsync(int groupId, int accountId, bool isAdmin = false)
        {
            // Kiểm tra trùng lặp
            var exists = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.AccountId == accountId);

            if (!exists)
            {
                var member = new GroupMember
                {
                    GroupId = groupId,
                    AccountId = accountId,
                    IsAdmin = isAdmin,
                    JoinedAt = DateTime.Now
                };
                await _context.GroupMembers.AddAsync(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsMemberOfGroupAsync(int groupId, int accountId)
        {
            return await _context.GroupMembers.AnyAsync(gm => gm.GroupId == groupId && gm.AccountId == accountId);
        }

        public async Task<ChatGroup?> GetGroupByIdAsync(int groupId)
        {
            // Lấy nhóm và thông tin thành viên nếu cần
            return await _context.ChatGroups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<List<ChatMessage2>> LoadGroupChatHistoryAsync(int groupId)
        {
            return await _context.ChatMessages2
                .Where(m => m.GroupId == groupId)
                .Include(m => m.Sender)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<List<ChatGroup>> GetGroupsByAccountIdAsync(int accountId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.AccountId == accountId)
                .Select(gm => gm.Group)
                .ToListAsync();
        }
        public async Task RemoveMemberFromGroupAsync(int groupId, int accountId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.AccountId == accountId);

            if (member != null)
            {
                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

    }
}
