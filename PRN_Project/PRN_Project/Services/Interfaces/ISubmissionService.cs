using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface ISubmissionService
    {
        // Học sinh nộp bài tập
        Task<bool> SubmitAssignmentAsync(int accountId, int postId, string content);

        // Lấy bài nộp của học sinh
        Task<AssignmentSubmission?> GetMySubmissionAsync(int accountId, int postId);

        // Cập nhật bài nộp của học sinh
        Task<bool> UpdateSubmissionAsync(int accountId, int postId, string content);

        // Lấy tất cả submissions của một post (cho giáo viên)
        Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByPostIdAsync(int postId);
    }
}

