using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.Interfaces;
using MediatR;

namespace CCMS.Application.Features.Bank.Batch.Commands
{
    public class TriggerBatchCommandHandler : IRequestHandler<TriggerBatchCommand>
    {
        private readonly IBatchService _batchService;

        public TriggerBatchCommandHandler(IBatchService batchService)
        {
            _batchService = batchService;
        }

        public async Task Handle(TriggerBatchCommand request, CancellationToken cancellationToken)
        {
            await _batchService.RunAsync(isManualRun: true);
        }
    }
}
