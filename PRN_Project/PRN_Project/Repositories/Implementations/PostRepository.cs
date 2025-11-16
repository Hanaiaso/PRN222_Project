using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Repositories.Implementations
{
    public class PostRepository : IPostRepository
    {
        private readonly LmsDbContext _context;

        public PostRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task AddPostAsync(Post newPost)
        {
            try
            {
                _context.Posts.Add(newPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu bài đăng: {ex.Message}", ex);
            }
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<IEnumerable<Post>> GetPostsByClassroomIdAsync(int classroomId)
        {
            return await _context.Posts
                .Where(p => p.ClassroomId == classroomId)
                .Include(p => p.Account)
                .OrderByDescending(p => p.CreateTime)
                .ToListAsync();
        }
    }
}

