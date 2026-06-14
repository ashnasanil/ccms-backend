using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CCMS.Application.Features.Bank.Cases.DTOs;
using CCMS.Application.Features.Bank.Cases.Queries;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;
using CCMS.Domain.Enums;

namespace CCMS.Application.Features.Bank.Cases.Handlers
{
    public class GetCasesByStatusHandler : IRequestHandler<GetCasesByStatusQuery, IEnumerable<CaseListDto>>
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCasesByStatusHandler(ICaseRepository caseRepository, ICurrentUserService currentUserService)
        {
            _caseRepository = caseRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<CaseListDto>> Handle(GetCasesByStatusQuery request, CancellationToken cancellationToken)
        {
            var org = _currentUserService.Organization;
            var cases = await _caseRepository.GetCasesForBankAsync(org!, null);

            var filteredCases = request.StatusCategory switch
            {
                "AwaitingAction" => cases.Where(c => c.Status == CaseStatus.AccountValidated),
                "PendingBatch" => cases.Where(c => c.Status == CaseStatus.Pending),
                "Completed" => cases.Where(c => c.Status == CaseStatus.FreezeApplied || c.Status == CaseStatus.BalanceProvided),
                "AutoResolved" => cases.Where(c => c.Status == CaseStatus.AccountNotFound),
                _ => cases
            };

            return filteredCases.Select(c => new CaseListDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                Status = c.Status,
                OrderType = c.OrderType,
                DefendantName = c.DefendantName
            });
        }
    }
}
