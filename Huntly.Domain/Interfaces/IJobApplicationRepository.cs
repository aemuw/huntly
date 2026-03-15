using Huntly.Domain.Entities;

namespace Huntly.Domain.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(Guid userId);
        Task AddAsync(JobApplication jobApplication);
        Task UpdateAsync(JobApplication jobApplication);
        Task DeleteAsync(Guid id);
    }
}
