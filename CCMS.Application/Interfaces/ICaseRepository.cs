using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces
{
    public interface ICaseRepository
    {
        Task<IEnumerable<Case>> GetAllAsync();
        Task<Case> GetByCaseNumberAsync(string caseNumber);
        Task<Case> AddAsync(Case newCase);
        
        // For the dashboard counts
        Task<int> GetTotalCountAsync();
        Task<int> GetCountByStatusAsync(CCMS.Domain.Enums.CaseStatus status);
        Task<int> GetCountByOrderTypeAsync(CCMS.Domain.Enums.OrderType orderType);
    }
}
