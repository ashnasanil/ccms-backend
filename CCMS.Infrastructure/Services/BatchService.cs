using System.Diagnostics;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Services;

public class BatchService : IBatchService
{
    private readonly ApplicationDbContext _context;

    public BatchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RunAsync(bool isManualRun)
    {
        var startTime = DateTime.UtcNow;

        var stopwatch = Stopwatch.StartNew();

        int validatedCount = 0;
        int notFoundCount = 0;

        var pendingCases = await _context.Cases
            .Where(x => x.Status == CaseStatus.Pending)
            .ToListAsync();

        foreach (var currentCase in pendingCases)
        {
            BankCustomer? customer = null;

            customer = await _context.BankCustomers
                .FirstOrDefaultAsync(x =>
                    x.AccountNumber == currentCase.AccountNumber);

            if (customer == null)
            {
                customer = await _context.BankCustomers
                    .FirstOrDefaultAsync(x =>
                        x.AadhaarNumber == currentCase.AadhaarNumber);
            }

            if (customer == null)
            {
                customer = await _context.BankCustomers
                    .FirstOrDefaultAsync(x =>
                        x.PanNumber == currentCase.PanNumber);
            }

            if (customer != null)
            {
                currentCase.Status = CaseStatus.AccountValidated;

                currentCase.MatchedAccountNumber =
                    customer.AccountNumber;

                currentCase.MatchedBalance =
                    customer.Balance;

                currentCase.MatchedAccountStatus =
                    customer.AccountStatus;

                validatedCount++;
            }
            else
            {
                currentCase.Status =
                    CaseStatus.AccountNotFound;

                notFoundCount++;
            }
        }

        await _context.SaveChangesAsync();

        stopwatch.Stop();

        _context.BatchJobLogs.Add(
            new BatchJobLog
            {
                StartedAt = startTime,
                EndedAt = DateTime.UtcNow,
                TotalProcessed = pendingCases.Count,
                ValidatedCount = validatedCount,
                NotFoundCount = notFoundCount,
                DurationMilliseconds = stopwatch.ElapsedMilliseconds,
                IsManualRun = isManualRun
            });

        await _context.SaveChangesAsync();
    }
}