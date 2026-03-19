using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Huntly.Infrastructure.Persistence.Repositories
{
    public class JobApplicationRepository : BaseRepository<JobApplication>, IJobApplicationRepository
    {
        public JobApplicationRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(Guid userId)
            => await _dbSet
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
