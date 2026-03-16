using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Huntly.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }
        public async Task<User?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> ExistsAsync(string email)
            => await _dbSet.AnyAsync(u => u.Email == email);
    }
}
