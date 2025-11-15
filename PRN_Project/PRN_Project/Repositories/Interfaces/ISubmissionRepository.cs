using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface ISubmissionRepository
    {
        // Thêm bài nộp của học sinh
        Task AddSubmissionAsync(AssignmentSubmission submission);

        // Kiểm tra học sinh đã nộp bài chưa
        Task<bool> HasStudentSubmittedAsync(int studentId, int postId);

        // Lấy bài nộp của học sinh
        Task<AssignmentSubmission?> GetStudentSubmissionAsync(int studentId, int postId);

        // Cập nhật bài nộp của học sinh
        Task UpdateSubmissionAsync(AssignmentSubmission submission);

        // Lấy tất cả submissions của một post
        Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByPostIdAsync(int postId);
    }
}

