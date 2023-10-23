using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UNAHUR.IoT.Business.Services;
using UNAHUR.IoT.DAL.Models;
using UNAHUR.IoT.FirmwareService.Storage;

namespace UNAHUR.IoT.FirmwareService.Controllers.v1
{
    /// <summary>
    /// Controlador que encapsula las acciones de formware para los dispositovos
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/firmware")]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    public class FirmwareController : ControllerBase
    {
        private readonly ILogger<FirmwareController> _log;
        private readonly CatalogService _catalogService;
        private readonly IFirmwareStorage _firmwareStorage;
        public FirmwareController(ILogger<FirmwareController> log,
            CatalogService catalogService,
            IFirmwareStorage firmwareStorage)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _firmwareStorage = firmwareStorage ?? throw new ArgumentNullException(nameof(firmwareStorage));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogItemsInfo>> Get(long id)
        {
            var item = await _catalogService.CatalogItemsInfo.Where(e => e.CatalogItemId == id).FirstOrDefaultAsync(Request.HttpContext.RequestAborted);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);

        }

        /// <summary>
        /// Uploads a firmware 
        /// </summary>
        /// <param name="grantorId"></param>
        /// <param name="interfaceName">Nombre de la interface a subir</param>
        /// <param name="uploadedFiles"></param>
        /// <returns>El grupo asignado para el tratamiento del archivo</returns>
        [HttpPut("{repo}/{tag}")]
        // IMPORTANTE: MultipartBodyLengthLimit debe estar alineado con el Kestrel.Limits.MaxRequestBodySize en el appsettings
        // esto es 250MB
        [RequestFormLimits(MultipartBodyLengthLimit = 262_144_000)]
        public async Task<ActionResult<string>> UploadAsync(string repo, string tag, [FromForm] IFormFile uploadedFile)
        {
            // estos datos deberian estar normalizados?
            // usuario de alta por ejemplo?
            // algun dato para relacionar con el modelo relacional.

            var tags = new Dictionary<string, string>
            {
                { "repo", repo },
                { "originalName", uploadedFile.FileName }
            };

            using (var stream = uploadedFile.OpenReadStream())
            {
                var firmwareModel = new FirmwareModel(repo, tag, uploadedFile.ContentType, stream);
                return Ok(await _firmwareStorage.UploadAsync(firmwareModel, tags, Request.HttpContext.RequestAborted));
            }
        }

        /// <summary>
        /// Uploads a firmware 
        /// </summary>
        /// <param name="grantorId"></param>
        /// <param name="interfaceName">Nombre de la interface a subir</param>
        /// <param name="uploadedFiles"></param>
        /// <returns>El grupo asignado para el tratamiento del archivo</returns>
        [HttpGet("{repo}/{tag}")]

        public async Task<IActionResult> DownloadAsync(string repo, string tag)
        {
            var file = await _firmwareStorage.DownloadAsync(repo, tag,  Request.HttpContext.RequestAborted);
            var content = file.DataStream;

            this.Response.ContentLength = content.Length;
            this.Response.Headers.Add("Accept-Ranges", "bytes");
            this.Response.Headers.Add("Content-Range", "bytes 0-" + content.Length);
            content.Position = 0;
            return File(content, file.ContentType, file.Name);
            
        }

        // TODO: EL ENDOPINT PARA LOS DISPOSITIVOS
        // DEBE TENER COMO ENTRADA SOLO EL ID DE DISPOSITIVO Y SE DEBE RESOLVER EL FIRMWARE CORRESPONDIENTE
        // LA AUTORIZACION DEBE SER CON EL SECRET DEL DISPOSIVO



        [HttpPut("test")]
        public async Task<ActionResult<bool>> TestAsync()
        {
            return Ok(await _firmwareStorage.TestAsync(Request.HttpContext.RequestAborted));
        }
    }
}