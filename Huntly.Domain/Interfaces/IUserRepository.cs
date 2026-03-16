using Huntly.Domain.Entities;

namespace Huntly.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(string email);
    }
}
