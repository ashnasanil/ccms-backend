using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Data;
using System.Linq;

namespace CCMS.Infrastructure.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private readonly AppDbContext _context;

        public CaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Case>> GetAllAsync()
        {
            return await _context.Cases.ToListAsync();
        }

        public async Task<Case> GetByCaseNumberAsync(string caseNumber)
        {
            return await _context.Cases.FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<Case> AddAsync(Case newCase)
        {
            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();
            return newCase;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Cases.CountAsync();
        }

        public async Task<int> GetCountByStatusAsync(CaseStatus status)
        {
            return await _context.Cases.CountAsync(c => c.Status == status);
        }

        public async Task<int> GetCountByOrderTypeAsync(OrderType orderType)
        {
            return await _context.Cases.CountAsync(c => c.OrderType == orderType);
        }
    }
}
