using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<Subject>> GetAllSubjectsAsync();
        Task<Subject?> GetSubjectByIdAsync(int id);
        Task AddSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);
        Task DeleteSubjectAsync(int id);
    }
}
