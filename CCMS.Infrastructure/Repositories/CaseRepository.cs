using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private readonly AppDbContext _context;

        public CaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Case?> GetByIdAsync(Guid id)
        {
            return await _context.Cases
                .Include(c => c.Attachments)
                .Include(c => c.CaseResponse)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Case?> GetByCaseNumberAsync(string caseNumber)
        {
            return await _context.Cases
                .Include(c => c.Attachments)
                .Include(c => c.CaseResponse)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<IEnumerable<Case>> GetCasesForBankAsync(string bankName, CaseStatus? status)
        {
            var query = _context.Cases.AsQueryable();

            if (!string.IsNullOrEmpty(bankName))
            {
                query = query.Where(c => c.BankName == bankName);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            return await query
                .Include(c => c.CaseResponse)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetPendingCasesAsync()
        {
            return await _context.Cases
                .Where(c => c.Status == CaseStatus.Pending)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetAllAsync()
        {
            return await _context.Cases
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetDailyCaseCountAsync(DateTime date)
        {
            var prefix = $"CCMS-{date:yyyyMMdd}-";
            var caseNumbers = await _context.Cases
                .Where(c => c.CaseNumber.StartsWith(prefix))
                .Select(c => c.CaseNumber)
                .ToListAsync();

            if (!caseNumbers.Any()) return 0;

            int maxSequence = 0;
            foreach (var num in caseNumbers)
            {
                if (num.Length > prefix.Length)
                {
                    var seqStr = num.Substring(prefix.Length);
                    if (int.TryParse(seqStr, out int seq) && seq > maxSequence)
                    {
                        maxSequence = seq;
                    }
                }
            }
            
            return maxSequence;
        }

        public async Task AddAsync(Case @case)
        {
            await _context.Cases.AddAsync(@case);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Case @case)
        {
            _context.Cases.Update(@case);
            await _context.SaveChangesAsync();
        }

        public async Task AddBatchLogAsync(BatchJobLog log)
        {
            await _context.BatchJobLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }


    }
}
