using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Implementations
{
    public class TeacherSubjectRepository : ITeacherSubjectRepository
    {
        private readonly LmsDbContext _context;

        public TeacherSubjectRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TeacherSubject teacherSubject)
        {
            await _context.TeacherSubjects.AddAsync(teacherSubject);
        }

        public void RemoveRange(IEnumerable<TeacherSubject> teacherSubjects)
        {
            _context.TeacherSubjects.RemoveRange(teacherSubjects);
        }
    }
}