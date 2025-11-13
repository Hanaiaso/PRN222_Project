using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using System.Linq;

namespace PRN_Project.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly LmsDbContext _context;

        public AccountRepository(LmsDbContext context)
        {
            _context = context;
        }

        public Account GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }

        public Account GetById(int id)
        {
            return _context.Accounts.FirstOrDefault(a => a.AId == id);
        }

        public bool EmailExists(string email)
        {
            return _context.Accounts.Any(a => a.Email == email);
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}