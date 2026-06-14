using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CCMS.Infrastructure.Services
{
    public class BatchValidationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BatchValidationService> _logger;

        public BatchValidationService(IServiceProvider serviceProvider, ILogger<BatchValidationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("BatchValidationService running at: {time}", DateTimeOffset.Now);

                try
                {
                    await ProcessPendingCasesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing BatchValidationService.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task ProcessPendingCasesAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var batchLog = new BatchJobLog
            {
                StartedAt = DateTime.UtcNow,
                Status = "Running"
            };
            
            context.BatchJobLogs.Add(batchLog);
            await context.SaveChangesAsync(stoppingToken);

            var pendingCases = await context.Cases
                .Where(c => c.Status == CaseStatus.Pending)
                .ToListAsync(stoppingToken);

            if (!pendingCases.Any())
            {
                batchLog.EndedAt = DateTime.UtcNow;
                batchLog.Status = "Completed - No Cases";
                await context.SaveChangesAsync(stoppingToken);
                return;
            }

            int processedCount = 0;
            int validatedCount = 0;
            int notFoundCount = 0;

            foreach (var @case in pendingCases)
            {
                var customer = await FindCustomerAsync(context, @case, stoppingToken);

                if (customer != null)
                {
                    @case.Status = CaseStatus.AccountValidated;
                    @case.MatchedAccountNumber = customer.AccountNumber;
                    @case.MatchedBalance = customer.Balance;
                    @case.MatchedAccountStatus = customer.AccountStatus;
                    validatedCount++;
                }
                else
                {
                    @case.Status = CaseStatus.AccountNotFound;
                    @case.CaseResponse = new CaseResponse
                    {
                        CaseId = @case.Id,
                        ResponseType = ResponseType.BalanceProvided,
                        Remarks = "No matching account found in bank records.",
                        RespondedBy = "System Batch Job",
                        RespondedAt = DateTime.UtcNow
                    };
                    notFoundCount++;
                }

                processedCount++;
            }

            batchLog.EndedAt = DateTime.UtcNow;
            batchLog.TotalProcessed = processedCount;
            batchLog.ValidatedCount = validatedCount;
            batchLog.NotFoundCount = notFoundCount;
            batchLog.DurationMilliseconds = (long)(batchLog.EndedAt.Value - batchLog.StartedAt).TotalMilliseconds;
            batchLog.Status = "Completed";

            await context.SaveChangesAsync(stoppingToken);
            
            _logger.LogInformation($"Batch processing completed. Processed: {processedCount}, Validated: {validatedCount}, Not Found: {notFoundCount}");
        }

        private async Task<BankCustomer?> FindCustomerAsync(AppDbContext context, Case @case, CancellationToken stoppingToken)
        {
            if (!string.IsNullOrWhiteSpace(@case.AccountNumber))
            {
                var match = await context.BankCustomers
                    .FirstOrDefaultAsync(c => c.AccountNumber == @case.AccountNumber, stoppingToken);
                if (match != null) return match;
            }

            if (!string.IsNullOrWhiteSpace(@case.AadhaarNumber))
            {
                var match = await context.BankCustomers
                    .FirstOrDefaultAsync(c => c.AadhaarNumber == @case.AadhaarNumber, stoppingToken);
                if (match != null) return match;
            }

            if (!string.IsNullOrWhiteSpace(@case.PanNumber))
            {
                var match = await context.BankCustomers
                    .FirstOrDefaultAsync(c => c.PanNumber == @case.PanNumber, stoppingToken);
                if (match != null) return match;
            }

            return null;
        }
    }
}
