using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        void Add(Student student);
        void SaveChanges();
    }
}