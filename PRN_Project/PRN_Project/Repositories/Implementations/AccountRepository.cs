using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly LmsDbContext _context;

        public AccountRepository(LmsDbContext context)
        {
            _context = context;
        }

        // === Triển khai Dùng cho Login/Register ===

        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Email == email);
        }

        public Account GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }

        // === Triển khai Dùng chung ===

        public async Task<Account> GetByIdAsync(int id)
        {
            // FindAsync là cách tốt nhất để lấy theo PK cho việc Cập nhật
            return await _context.Accounts.FindAsync(id);
        }
        public Account GetById(int id)
        {
            // Dùng Find() là cách tốt nhất để lấy theo PK
            return _context.Accounts.Find(id);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void Remove(Account account)
        {
            _context.Accounts.Remove(account);
        }
        public bool EmailExists(string email)
        {
            return _context.Accounts.Any(a => a.Email == email);
        }

        // Triển khai từ interface của bạn
        public void Add(Account account)
        {
            _context.Accounts.Add(account);
        }



        // Triển khai từ interface của bạn
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // === Triển khai Dùng cho Quản lý Học sinh ===

        public async Task<List<Account>> GetStudentAccountsWithDetailsAsync()
        {
            return await _context.Accounts
                .Where(a => a.Role == RoleType.Student)
                .Include(a => a.Student) // Nối bảng Student
                .AsNoTracking()
                .OrderBy(a => a.Student.SName)
                .ToListAsync();
        }
    }
}