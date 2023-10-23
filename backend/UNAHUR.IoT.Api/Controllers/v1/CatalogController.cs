using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UNAHUR.IoT.Business.Services;
using UNAHUR.IoT.DAL.Models;

namespace UNAHUR.IoT.Api.Controllers.v1
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/catalog")]
    [ApiVersion("1.0")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _log;
        private readonly CatalogService _catalogService;
        public CatalogController(ILogger<CatalogController> log, CatalogService catalogService)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
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
    }
}