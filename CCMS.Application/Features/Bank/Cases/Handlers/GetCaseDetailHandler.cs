using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CCMS.Application.Features.Bank.Cases.DTOs;
using CCMS.Application.Features.Bank.Cases.Queries;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;

namespace CCMS.Application.Features.Bank.Cases.Handlers
{
    public class GetCaseDetailHandler : IRequestHandler<GetCaseDetailQuery, CaseDetailDto>
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMaskingService _maskingService;

        public GetCaseDetailHandler(ICaseRepository caseRepository, ICurrentUserService currentUserService, IMaskingService maskingService)
        {
            _caseRepository = caseRepository;
            _currentUserService = currentUserService;
            _maskingService = maskingService;
        }

        public async Task<CaseDetailDto> Handle(GetCaseDetailQuery request, CancellationToken cancellationToken)
        {
            var @case = await _caseRepository.GetByIdAsync(request.CaseId);
            if (@case == null || @case.BankName != _currentUserService.Organization)
            {
                throw new UnauthorizedAccessException("Case not found or access denied.");
            }

            return new CaseDetailDto
            {
                Id = @case.Id,
                CaseNumber = @case.CaseNumber,
                Status = @case.Status,
                OrderType = @case.OrderType,
                DefendantName = @case.DefendantName,
                DefendantAadhaar = _maskingService.MaskAadhaar(@case.AadhaarNumber),
                DefendantPAN = _maskingService.MaskPAN(@case.PanNumber),
                DefendantAccountNumber = _maskingService.MaskAccountNumber(@case.AccountNumber),
                DefendantBankName = @case.BankName,
                MatchedAccountNumber = string.IsNullOrEmpty(@case.MatchedAccountNumber) ? "" : _maskingService.MaskAccountNumber(@case.MatchedAccountNumber),
                MatchedBalance = @case.MatchedBalance,
                MatchedAccountStatus = @case.MatchedAccountStatus,
                Attachments = @case.Attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FilePath = a.StoragePath
                }).ToList(),
                ExistingResponse = @case.CaseResponse == null ? null : new CaseResponseDto
                {
                    Id = @case.CaseResponse.Id,
                    ResponseType = @case.CaseResponse.ResponseType,
                    FreezeAmount = @case.CaseResponse.FreezeAmount,
                    BalanceAmount = @case.CaseResponse.BalanceAmount,
                    Remarks = @case.CaseResponse.Remarks,
                    RespondedBy = @case.CaseResponse.RespondedBy,
                    RespondedAt = @case.CaseResponse.RespondedAt
                }
            };
        }
    }
}
