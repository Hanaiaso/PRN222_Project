using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Repositories.Implementations
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly LmsDbContext _context;

        public SubmissionRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task AddSubmissionAsync(AssignmentSubmission submission)
        {
            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasStudentSubmittedAsync(int studentId, int postId)
        {
            return await _context.AssignmentSubmissions
                .AnyAsync(s => s.Sid == studentId && s.PostId == postId);
        }

        public async Task<AssignmentSubmission?> GetStudentSubmissionAsync(int studentId, int postId)
        {
            return await _context.AssignmentSubmissions
                .Include(s => s.Student)
                    .ThenInclude(st => st.Account)
                .Include(s => s.Post)
                .FirstOrDefaultAsync(s => s.Sid == studentId && s.PostId == postId);
        }

        public async Task UpdateSubmissionAsync(AssignmentSubmission submission)
        {
            _context.AssignmentSubmissions.Update(submission);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByPostIdAsync(int postId)
        {
            return await _context.AssignmentSubmissions
                .Where(s => s.PostId == postId)
                .Include(s => s.Student)
                    .ThenInclude(st => st.Account)
                .OrderByDescending(s => s.SubmitTime)
                .ToListAsync();
        }
    }
}

