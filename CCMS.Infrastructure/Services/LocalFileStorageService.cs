using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CCMS.Infrastructure.Services
{
    public interface ILocalFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }

    public class LocalFileStorageService : ILocalFileStorageService
    {
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Attachments");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
