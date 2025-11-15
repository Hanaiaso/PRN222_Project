using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;

        public PostService(IPostRepository postRepo)
        {
            _postRepo = postRepo;
        }

        public async Task<Post> CreatePostAsync(int classroomId, int accountId, string title, string content, string postType, DateTime? dueDate)
        {
            var newPost = new Post
            {
                ClassroomId = classroomId,
                Aid = accountId,
                Title = title,
                Content = content,
                PostType = postType,
                DueDate = dueDate,
                CreateTime = DateTime.Now
            };

            await _postRepo.AddPostAsync(newPost);
            return newPost;
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _postRepo.GetPostByIdAsync(postId);
        }

        public async Task<IEnumerable<Post>> GetPostsByClassroomIdAsync(int classroomId)
        {
            return await _postRepo.GetPostsByClassroomIdAsync(classroomId);
        }
    }
}

