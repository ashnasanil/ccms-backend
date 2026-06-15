using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;

namespace CCMS.Application.Interfaces.Repositories
{
    public interface ICaseRepository
    {
        Task<Case?> GetByIdAsync(Guid id);
        Task<Case?> GetByCaseNumberAsync(string caseNumber);
        Task<IEnumerable<Case>> GetCasesForBankAsync(string bankName, CaseStatus? status);
        Task<IEnumerable<Case>> GetPendingCasesAsync();
        Task<IEnumerable<Case>> GetAllAsync();
        Task<int> GetDailyCaseCountAsync(DateTime date);
        Task AddAsync(Case @case);
        Task UpdateAsync(Case @case);
        Task AddBatchLogAsync(BatchJobLog log);
    }
}
