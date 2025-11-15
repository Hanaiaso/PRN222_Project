using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces; // Thêm
using PRN_Project.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Services.Implementations
{
    public class TeacherClassroomService : ITeacherClassroomService
    {
        // Service phụ thuộc vào Repository
        private readonly ITeacherClassroomRepository _repo;

        public TeacherClassroomService(ITeacherClassroomRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Classroom>> GetClassroomsByTeacherAccountIdAsync(int accountId)
        {
            var teacher = await _repo.GetTeacherByAccountIdAsync(accountId);
            if (teacher == null)
            {
                return new List<Classroom>(); // Trả về ds rỗng nếu không tìm thấy teacher
            }
            return await _repo.GetClassroomsByTeacherIdAsync(teacher.TId);
        }

        public async Task<ClassroomDetailsDTO> GetClassroomDetailsAsync(int classroomId)
        {
            var classroom = await _repo.GetClassroomByIdAsync(classroomId);
            if (classroom == null) return null;

            var members = await _repo.GetMembersByClassroomIdAsync(classroomId);

            return new ClassroomDetailsDTO
            {
                ClassroomName = classroom.ClassName,
                Members = members
            };
        }

        public async Task<StudentSubmissionsDTO> GetStudentSubmissionsAsync(int studentId)
        {
            var student = await _repo.GetStudentByIdAsync(studentId);
            if (student == null) return null;

            var submissions = await _repo.GetSubmissionsByStudentIdAsync(studentId);

            return new StudentSubmissionsDTO
            {
                StudentName = student.SName,
                Submissions = submissions
            };
        }

        public async Task<Submit> GetSubmissionForEditAsync(int submitId)
        {
            return await _repo.GetSubmissionForEditAsync(submitId);
        }

        public async Task<Submit> UpdateSubmissionCommentAsync(int submitId, string comment)
        {
            var submission = await _repo.GetSubmissionByIdAsync(submitId);
            if (submission == null) return null;

            submission.Comment = comment;

            _repo.UpdateSubmission(submission); // Dùng Update (Sync)
            await _repo.SaveChangesAsync(); // Dùng Save (Async)

            return submission;
        }
    }
}