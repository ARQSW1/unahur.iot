using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public interface IFirmwareStorage
    {
        Task<string> UploadAsync(string repo, string tag, IFormFile file, IDictionary<string, string> tags = null, CancellationToken cancelationToken = default);
        Task<bool> TestAsync(CancellationToken cancelationToken = default);
    }
}
