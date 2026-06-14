using System.Threading.Tasks;
using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces.Repositories
{
    public interface ICaseResponseRepository
    {
        Task AddAsync(CaseResponse caseResponse);
    }
}
