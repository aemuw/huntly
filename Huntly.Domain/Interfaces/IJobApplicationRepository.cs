using Huntly.Domain.Entities;

namespace Huntly.Domain.Interfaces
{
    public interface IJobApplicationRepository : IRepository<JobApplication>
    {
        Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid id);
    }
}
