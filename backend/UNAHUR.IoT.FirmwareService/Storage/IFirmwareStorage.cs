using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public interface IFirmwareStorage
    {
        Task<string> UploadAsync(string repo, string tag, IFormFile file, CancellationToken cancelationToken = default);
    }
}
