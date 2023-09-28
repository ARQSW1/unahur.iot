using Microsoft.AspNetCore.Authorization;
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
    [Route("api/v{version:apiVersion}/catalog")]
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
    }
}