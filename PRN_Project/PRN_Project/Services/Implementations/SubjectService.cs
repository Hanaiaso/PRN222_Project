using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repo;
        public SubjectService(ISubjectRepository repo) => _repo = repo;

        public Task AddSubjectAsync(Subject subject) => _repo.AddAsync(subject);
        public Task DeleteSubjectAsync(int id) => _repo.DeleteAsync(id);
        public Task<List<Subject>> GetAllSubjectsAsync() => _repo.GetAllAsync();
        public Task<Subject?> GetSubjectByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task UpdateSubjectAsync(Subject subject) => _repo.UpdateAsync(subject);
    }
}
