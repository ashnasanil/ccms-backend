using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CCMS.Application.Features.Bank.Responses.Commands;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;

namespace CCMS.Application.Features.Bank.Responses.Handlers
{
    public class SubmitBalanceResponseHandler : IRequestHandler<SubmitBalanceResponseCommand>
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICurrentUserService _currentUserService;

        public SubmitBalanceResponseHandler(ICaseRepository caseRepository, ICurrentUserService currentUserService)
        {
            _caseRepository = caseRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(SubmitBalanceResponseCommand request, CancellationToken cancellationToken)
        {
            var @case = await _caseRepository.GetByIdAsync(request.CaseId);
            if (@case == null || @case.BankName != _currentUserService.Organization)
            {
                throw new UnauthorizedAccessException("Case not found or access denied.");
            }

            if (@case.Status != CaseStatus.AccountValidated)
            {
                throw new InvalidOperationException("Response allowed only when case is AccountValidated.");
            }

            if (@case.CaseResponse != null || @case.Status == CaseStatus.FreezeApplied || @case.Status == CaseStatus.BalanceProvided || @case.Status == CaseStatus.AccountNotFound)
            {
                throw new InvalidOperationException("A case may only receive one response, and cannot be modified if in a terminal state.");
            }

            @case.CaseResponse = new CaseResponse
            {
                CaseId = @case.Id,
                ResponseType = ResponseType.BalanceProvided,
                BalanceAmount = request.BalanceAmount,
                Remarks = request.Remarks,
                RespondedBy = _currentUserService.UserId ?? "Unknown",
                RespondedAt = DateTime.UtcNow
            };

            @case.Status = CaseStatus.BalanceProvided;
            await _caseRepository.UpdateAsync(@case);
        }
    }
}
