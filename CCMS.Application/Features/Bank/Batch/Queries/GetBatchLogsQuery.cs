using System.Collections.Generic;
using CCMS.Application.DTOs.Bank;
using MediatR;

namespace CCMS.Application.Features.Bank.Batch.Queries
{
    public record GetBatchLogsQuery : IRequest<IEnumerable<BatchLogDto>>;
}
