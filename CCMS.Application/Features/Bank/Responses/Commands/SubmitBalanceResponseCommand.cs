using System;
using MediatR;

namespace CCMS.Application.Features.Bank.Responses.Commands
{
    public record SubmitBalanceResponseCommand(Guid CaseId, decimal BalanceAmount, string Remarks) : IRequest;
}
