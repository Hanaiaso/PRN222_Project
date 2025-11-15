using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Services.Interfaces
{
    // Tạo các DTO (Data Transfer Object) nhỏ để Service trả về dữ liệu sạch
    public class ClassroomDetailsDTO
    {
        public string ClassroomName { get; set; }
        public List<ClassroomMember> Members { get; set; }
    }

    public class StudentSubmissionsDTO
    {
        public string StudentName { get; set; }
        public List<Submit> Submissions { get; set; }
    }

    // Interface
    public interface ITeacherClassroomService
    {
        Task<List<Classroom>> GetClassroomsByTeacherAccountIdAsync(int accountId);
        Task<ClassroomDetailsDTO> GetClassroomDetailsAsync(int classroomId);
        Task<StudentSubmissionsDTO> GetStudentSubmissionsAsync(int studentId);
        Task<Submit> GetSubmissionForEditAsync(int submitId);
        Task<Submit> UpdateSubmissionCommentAsync(int submitId, string comment);
    }
}