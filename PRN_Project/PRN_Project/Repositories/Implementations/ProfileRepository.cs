using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using System.Threading.Tasks;

namespace PRN_Project.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly LmsDbContext _context;
        public ProfileRepository(LmsDbContext context)
        {
            _context = context;
        }

        // --- Nơi chứa logic Include ---
        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.AId == accountId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // Trả về true nếu có ít nhất 1 dòng được lưu
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}