using System.Threading.Tasks;
using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces.Repositories
{
    public interface IBatchJobRepository
    {
        Task AddBatchLogAsync(BatchJobLog log);
        Task<System.Collections.Generic.IEnumerable<BatchJobLog>> GetBatchLogsAsync();
    }
}
