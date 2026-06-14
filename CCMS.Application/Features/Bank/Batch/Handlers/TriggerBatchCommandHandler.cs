using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CCMS.Application.Features.Bank.Batch.Commands;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;

namespace CCMS.Application.Features.Bank.Batch.Handlers
{
    public class TriggerBatchCommandHandler : IRequestHandler<TriggerBatchCommand>
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IBatchJobRepository _batchJobRepository;
        private readonly IBankAccountVerificationService _bankAccountVerificationService;

        public TriggerBatchCommandHandler(ICaseRepository caseRepository, IBatchJobRepository batchJobRepository, IBankAccountVerificationService bankAccountVerificationService)
        {
            _caseRepository = caseRepository;
            _batchJobRepository = batchJobRepository;
            _bankAccountVerificationService = bankAccountVerificationService;
        }

        public async Task Handle(TriggerBatchCommand request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var pendingCases = await _caseRepository.GetPendingCasesAsync();

            int processedCount = 0;
            int validatedCount = 0;
            int notFoundCount = 0;

            foreach (var @case in pendingCases)
            {
                if (@case.Status != CaseStatus.Pending)
                    continue;

                processedCount++;

                var result = await _bankAccountVerificationService.VerifyAccountAsync(
                    @case.DefendantAccountNumber,
                    @case.DefendantAadhaar,
                    @case.DefendantPAN);

                if (result.IsMatch)
                {
                    @case.Status = CaseStatus.AccountValidated;
                    @case.MatchedAccountNumber = result.AccountNumber;
                    @case.MatchedBalance = result.Balance;
                    @case.MatchedAccountStatus = result.Status;
                    validatedCount++;
                }
                else
                {
                    @case.Status = CaseStatus.AccountNotFound;
                    @case.CaseResponse = new CaseResponse
                    {
                        CaseId = @case.Id,
                        ResponseType = ResponseType.BalanceProvided, // Default
                        Remarks = "No matching account found in bank records.",
                        RespondedBy = "System",
                        RespondedAt = DateTime.UtcNow
                    };
                    notFoundCount++;
                }

                await _caseRepository.UpdateAsync(@case);
            }

            sw.Stop();

            var batchLog = new BatchJobLog
            {
                ProcessedCount = processedCount,
                ValidatedCount = validatedCount,
                NotFoundCount = notFoundCount,
                DurationMs = sw.ElapsedMilliseconds,
                StartedAt = DateTime.UtcNow.AddMilliseconds(-sw.ElapsedMilliseconds),
                EndedAt = DateTime.UtcNow,
                Status = "Completed"
            };

            await _batchJobRepository.AddBatchLogAsync(batchLog);
        }
    }
}
