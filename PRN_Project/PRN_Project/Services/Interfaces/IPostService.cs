using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface IPostService
    {
        // Tạo bài đăng mới (thông báo hoặc bài tập)
        Task<Post> CreatePostAsync(int classroomId, int accountId, string title, string content, string postType, DateTime? dueDate);

        // Lấy Post theo ID
        Task<Post?> GetPostByIdAsync(int postId);

        // Lấy tất cả Posts của một lớp học
        Task<IEnumerable<Post>> GetPostsByClassroomIdAsync(int classroomId);
    }
}

