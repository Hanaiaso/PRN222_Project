using PRN_Project.Models;
using System.Linq;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetClassesByTeacherIdAsync(int teacherId);

        Task<IEnumerable<Classroom>> GetClassesByStudentIdAsync(int studentId);

        Task<Classroom> GetClassByCodeAsync(string classCode);

        Task<bool> ClassCodeExistsAsync(string classCode);

        Task AddClassAsync(Classroom newClass);

        Task AddMemberAsync(ClassroomMember newMember);

        Task<bool> IsStudentInClassAsync(int studentId, int classroomId);

        Task<Classroom> GetClassByIdAsync(int classroomId);
    }
}
