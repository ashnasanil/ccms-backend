using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CCMS.Application.Interfaces.Services
{
    public interface ILocalFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
