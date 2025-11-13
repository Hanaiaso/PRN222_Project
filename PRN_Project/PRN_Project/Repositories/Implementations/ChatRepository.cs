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
    }
}
