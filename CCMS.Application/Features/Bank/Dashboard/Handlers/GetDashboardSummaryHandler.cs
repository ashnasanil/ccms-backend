using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CCMS.Application.Features.Bank.Dashboard.DTOs;
using CCMS.Application.Features.Bank.Dashboard.Queries;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;
using CCMS.Domain.Enums;

namespace CCMS.Application.Features.Bank.Dashboard.Handlers
{
    public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetDashboardSummaryHandler(ICaseRepository caseRepository, ICurrentUserService currentUserService)
        {
            _caseRepository = caseRepository;
            _currentUserService = currentUserService;
        }

        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var org = _currentUserService.Organization;
            var cases = await _caseRepository.GetCasesForBankAsync(org!, null);

            return new DashboardSummaryDto
            {
                AwaitingAction = cases.Count(c => c.Status == CaseStatus.AccountValidated),
                PendingBatch = cases.Count(c => c.Status == CaseStatus.Pending),
                Completed = cases.Count(c => c.Status == CaseStatus.FreezeApplied || c.Status == CaseStatus.BalanceProvided),
                AutoResolved = cases.Count(c => c.Status == CaseStatus.AccountNotFound)
            };
        }
    }
}
