using System.Threading.Tasks;
using CCMS.Application.DTOs.Court;

namespace CCMS.Application.Interfaces
{
    public interface ICourtService
    {
        Task<CourtDashboardDto> GetDashboardAsync();
    }
}
