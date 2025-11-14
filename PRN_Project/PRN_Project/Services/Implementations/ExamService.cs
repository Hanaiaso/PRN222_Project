using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;

        public ExamService(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _examRepository.GetAllAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int id)
        {
            return await _examRepository.GetByIdAsync(id);
        }

        public async Task CreateExamAsync(Exam exam)
        {
            await _examRepository.AddAsync(exam);
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            await _examRepository.UpdateAsync(exam);
        }

        public async Task DeleteExamAsync(Exam exam)
        {
            await _examRepository.DeleteAsync(exam);
        }
    }
}