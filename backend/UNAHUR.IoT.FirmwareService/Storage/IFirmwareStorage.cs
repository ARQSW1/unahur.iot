using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public interface IFirmwareStorage
    {
        // <summary>
        /// Sube un archivo al repositorio <paramref name="repo"/> el archivo especificado en <paramref name="file"/>
        /// </summary>
        ///<param name="file">Archivo de firmware a subir</param>
        /// <param name="tags">Tags del archivo</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation </param>
        /// <exception cref="FluentValidation.ValidationException">En caso de algun problema con el nombre del repo o del archivo</exception>
        /// <returns>The object ID</returns>
        Task<string> UploadAsync(
            FirmwareModel file,
            IDictionary<string, string> tags = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene un archivo del storage y lo coloca en <paramref name="outputStream"/>
        /// </summary>
        /// <param name="repo">Nombre del repositorio</param>
        /// <param name="name">Nombre del archivo</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation </param>
        /// <returns>Un objeto <see cref="FirmwareModel"/> con los datos del firmware</returns>
        Task<FirmwareModel> DownloadAsync(
            string repo,
            string name,
            CancellationToken cancellationToken = default);

            /// <summary>
            /// Intenta conectarse al storage
            /// </summary>
            /// <param name="cancellationToken">Optional cancellation token to cancel the operation </param>
            /// <returns><code>true</code> si la conexion exs exitosa, <code>false</code> si la conexion no es exitosa</returns>
            /// <remarks>Se utiliza el log para registrar cualquier error en la conexion</remarks>
            Task<bool> TestAsync(CancellationToken cancellationToken = default);
    }
}
