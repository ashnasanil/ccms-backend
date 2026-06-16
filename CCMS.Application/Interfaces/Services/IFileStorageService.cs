using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CCMS.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
