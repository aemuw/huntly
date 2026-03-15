using Huntly.Domain.Entities;

namespace Huntly.Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Company>> GetAllAsync();
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
    }
}
