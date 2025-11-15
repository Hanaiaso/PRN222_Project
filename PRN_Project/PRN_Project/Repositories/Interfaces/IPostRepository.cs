using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IPostRepository
    {
        // Thêm bài đăng mới vào lớp học
        Task AddPostAsync(Post newPost);

        // Lấy Post theo ID
        Task<Post?> GetPostByIdAsync(int postId);

        // Lấy tất cả Posts của một lớp học
        Task<IEnumerable<Post>> GetPostsByClassroomIdAsync(int classroomId);
    }
}

