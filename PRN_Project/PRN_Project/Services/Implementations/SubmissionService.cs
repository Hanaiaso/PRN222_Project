using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Services.Implementations
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IPostRepository _postRepo;
        private readonly LmsDbContext _context;

        public SubmissionService(ISubmissionRepository submissionRepo, IPostRepository postRepo, LmsDbContext context)
        {
            _submissionRepo = submissionRepo;
            _postRepo = postRepo;
            _context = context;
        }

        public async Task<bool> SubmitAssignmentAsync(int accountId, int postId, string content)
        {
            // Chuyển đổi AccountId sang StudentId
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.AId == accountId);
            
            if (student == null) return false;

            // Kiểm tra Post có phải Assignment không
            var post = await _postRepo.GetPostByIdAsync(postId);
            
            if (post == null || post.PostType != "Assignment") return false;

            // Kiểm tra đã nộp chưa (nếu đã nộp thì không cho nộp lại, hoặc có thể cho phép sửa)
            if (await _submissionRepo.HasStudentSubmittedAsync(student.SId, postId))
            {
                return false; // Đã nộp rồi
            }

            // Tạo bài nộp
            var submission = new AssignmentSubmission
            {
                PostId = postId,
                Sid = student.SId,
                Content = content,
                SubmitTime = DateTime.Now,
                Status = "Submitted"
            };

            await _submissionRepo.AddSubmissionAsync(submission);
            return true;
        }

        public async Task<AssignmentSubmission?> GetMySubmissionAsync(int accountId, int postId)
        {
            // Chuyển đổi AccountId sang StudentId
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.AId == accountId);
            
            if (student == null) return null;

            return await _submissionRepo.GetStudentSubmissionAsync(student.SId, postId);
        }

        public async Task<bool> UpdateSubmissionAsync(int accountId, int postId, string content)
        {
            // Chuyển đổi AccountId sang StudentId
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.AId == accountId);
            
            if (student == null) return false;

            // Kiểm tra Post có phải Assignment không
            var post = await _postRepo.GetPostByIdAsync(postId);
            
            if (post == null || post.PostType != "Assignment") return false;

            // Kiểm tra đã nộp chưa
            var existingSubmission = await _submissionRepo.GetStudentSubmissionAsync(student.SId, postId);
            if (existingSubmission == null) return false;

            // Kiểm tra deadline - chỉ cho phép sửa nếu chưa quá hạn
            if (post.DueDate.HasValue && post.DueDate.Value < DateTime.Now)
            {
                return false; // Quá hạn, không cho sửa
            }

            // Cập nhật nội dung
            existingSubmission.Content = content;
            existingSubmission.SubmitTime = DateTime.Now; // Cập nhật thời gian nộp

            await _submissionRepo.UpdateSubmissionAsync(existingSubmission);
            return true;
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByPostIdAsync(int postId)
        {
            return await _submissionRepo.GetSubmissionsByPostIdAsync(postId);
        }
    }
}

