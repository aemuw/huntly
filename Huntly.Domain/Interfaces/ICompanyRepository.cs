using Huntly.Domain.Entities;

namespace Huntly.Domain.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<IReadOnlyList<Company>> GetAllAsync();
    }
}
