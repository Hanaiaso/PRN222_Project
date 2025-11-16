using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Interfaces
{
    public interface ITeacherClassroomRepository
    {
        // Các phương thức cho Teacher
        Task<Teacher> GetTeacherByAccountIdAsync(int accountId);

        // Các phương thức cho Classroom
        Task<List<Classroom>> GetClassroomsByTeacherIdAsync(int teacherId);
        Task<Classroom> GetClassroomByIdAsync(int classroomId);
        Task<List<ClassroomMember>> GetMembersByClassroomIdAsync(int classroomId);

        // Các phương thức cho Student
        Task<Student> GetStudentByIdAsync(int studentId);

        // Các phương thức cho Submit
        Task<List<Submit>> GetSubmissionsByStudentIdAsync(int studentId);
        Task<Submit> GetSubmissionForEditAsync(int submitId);
        Task<Submit> GetSubmissionByIdAsync(int submitId); // Lấy bản ghi để update
        void UpdateSubmission(Submit submission); // Chỉ update, không save
        Task<int> SaveChangesAsync(); // Hàm save riêng

        Task<Submit> GetSubmissionWithDetailsAsync(int submitId);
    }
}