using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        // === Dùng cho Login/Register ===
        Task<Account> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);

        // === Dùng chung ===
        Task<Account> GetByIdAsync(int id);
        Task AddAsync(Account account);
        void Update(Account account); // Update/Remove thường là sync
        void Remove(Account account);
        Task<int> SaveChangesAsync();

        Account GetByEmail(string email);
        Account GetById(int id);



        bool EmailExists(string email);
        void Add(Account account);
        void SaveChanges();

        // === Dùng cho Quản lý Học sinh (Student Management) ===
        Task<List<Account>> GetStudentAccountsWithDetailsAsync();
    }
}