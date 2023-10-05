using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UNAHUR.IoT.Business.Services;
using UNAHUR.IoT.DAL.MOdels;

namespace UNAHUR.IoT.FirmwareService.Controllers.v1
{
    /// <summary>
    /// Controlador que encapsula las acciones de formware para los dispositovos
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/firmware")]
    [ApiVersion("1.0")]
    public class FirmwareController : ControllerBase
    {
        private readonly ILogger<FirmwareController> _log;
        private readonly CatalogService _catalogService;
        public FirmwareController(ILogger<FirmwareController> log, CatalogService catalogService)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        }

        /// <summary>
        /// Retorna informacion sobre un repositorio puntual
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// Uploads a collections of fo�eInfo 
        /// </summary>
        /// <param name="grantorId"></param>
        /// <param name="interfaceName">Nombre de la interface a subir</param>
        /// <param name="uploadedFiles"></param>
        /// <returns>El grupo asignado para el tratamiento del archivo</returns>
        [HttpPut("{repo}/{tag}")]
        // IMPORTANTE: MultipartBodyLengthLimit debe estar alineado con el Kestrel.Limits.MaxRequestBodySize en el appsettings
        // esto es 250MB
        [RequestFormLimits(MultipartBodyLengthLimit = 262_144_000)]
        public async Task<ActionResult<long>> UploadAsync(string repo, string tag, [FromForm] IFormFile uploadedFile)
        {
            // TODO: verificar si el usuario puede subir al grantorId

            

            return Ok(1);
        }
    }
}