using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface IClassroomService
    {
        // Lấy danh sách lớp học cho người dùng hiện tại (tự động
        // kiểm tra vai trò là Student hay Teacher)
        Task<IEnumerable<Classroom>> GetMyClassesAsync(int userId, string userRole);

        // Học sinh tham gia lớp
        Task<bool> JoinClassAsync(int studentId, string classCode);

        // Giáo viên tạo lớp
        Task<Classroom> CreateClassAsync(string className, string description, int teacherId);

        // Lấy lớp học theo ID
        Task<Classroom> GetClassByIdAsync(int classroomId);
    }
}
