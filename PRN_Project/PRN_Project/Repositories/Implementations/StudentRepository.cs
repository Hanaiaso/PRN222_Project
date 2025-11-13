using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly LmsDbContext _context;

        public StudentRepository(LmsDbContext context)
        {
            _context = context;
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}