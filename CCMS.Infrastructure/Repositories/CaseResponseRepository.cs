using System.Threading.Tasks;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Domain.Entities;
using CCMS.Infrastructure.Persistence;

namespace CCMS.Infrastructure.Repositories
{
    public class CaseResponseRepository : ICaseResponseRepository
    {
        private readonly AppDbContext _context;

        public CaseResponseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CaseResponse caseResponse)
        {
            await _context.CaseResponses.AddAsync(caseResponse);
            await _context.SaveChangesAsync();
        }
    }
}
