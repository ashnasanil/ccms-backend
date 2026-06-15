using System;
using MediatR;

namespace CCMS.Application.Features.Bank.Responses.Commands
{
    public record SubmitFreezeResponseCommand(Guid CaseId, decimal FreezeAmount, string Remarks) : IRequest;
}
