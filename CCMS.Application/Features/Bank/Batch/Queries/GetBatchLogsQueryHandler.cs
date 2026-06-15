using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.DTOs.Bank;
using CCMS.Application.Interfaces.Repositories;
using MediatR;

namespace CCMS.Application.Features.Bank.Batch.Queries
{
    public class GetBatchLogsQueryHandler : IRequestHandler<GetBatchLogsQuery, IEnumerable<BatchLogDto>>
    {
        private readonly IBatchJobRepository _repository;

        public GetBatchLogsQueryHandler(IBatchJobRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BatchLogDto>> Handle(GetBatchLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repository.GetBatchLogsAsync();
            return logs.Select(log => new BatchLogDto
            {
                Id = log.Id,
                StartedAt = log.StartedAt,
                EndedAt = log.EndedAt,
                TotalProcessed = log.TotalProcessed,
                ValidatedCount = log.ValidatedCount,
                NotFoundCount = log.NotFoundCount,
                DurationMilliseconds = log.DurationMilliseconds,
                IsManualRun = log.IsManualRun
            });
        }
    }
}
