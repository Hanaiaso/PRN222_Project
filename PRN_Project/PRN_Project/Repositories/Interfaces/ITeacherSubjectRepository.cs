using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Interfaces
{
    public interface ITeacherSubjectRepository
    {
        Task AddAsync(TeacherSubject teacherSubject);
        void RemoveRange(IEnumerable<TeacherSubject> teacherSubjects);
    }
}