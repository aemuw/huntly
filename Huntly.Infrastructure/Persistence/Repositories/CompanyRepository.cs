using Huntly.Domain.Entities;
using Huntly.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Huntly.Infrastructure.Persistence.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext context) : base(context) { }
        public async Task<IReadOnlyList<Company>> GetAllAsync()
            => await _dbSet
            .Include(c => c.Technologies)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
