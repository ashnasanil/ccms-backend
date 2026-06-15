using System.Threading.Tasks;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Domain.Entities;
using CCMS.Infrastructure.Persistence;

namespace CCMS.Infrastructure.Repositories
{
    public class BatchJobRepository : IBatchJobRepository
    {
        private readonly AppDbContext _context;

        public BatchJobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddBatchLogAsync(BatchJobLog log)
        {
            await _context.BatchJobLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<System.Collections.Generic.IEnumerable<BatchJobLog>> GetBatchLogsAsync()
        {
            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                System.Linq.Queryable.OrderByDescending(_context.BatchJobLogs, x => x.StartedAt)
            );
        }
    }
}
