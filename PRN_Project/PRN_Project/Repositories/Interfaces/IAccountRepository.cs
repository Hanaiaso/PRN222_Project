using PRN_Project.Models;
using System.Linq;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Account GetByEmail(string email);
        Account GetById(int id);
        bool EmailExists(string email);
        void Add(Account account);
        void Update(Account account);
        void SaveChanges();
    }
}