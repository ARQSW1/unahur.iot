using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNAHUR.IoT.DAL.Context;
using UNAHUR.IoT.DAL.MOdels;

namespace UNAHUR.IoT.Business.Services
{
    /// <summary>
    /// Encapsula la funcionalidad para manupular el catalogo de items
    /// </summary>
    public class CatalogService
    {
        private readonly ILogger _log;
        private readonly IoTContext _context;

        /// <summary>
        /// Crea una instancia del servicio
        /// </summary>
        /// <param name="log"></param>
        /// <param name="context"></param>
        public CatalogService(
            ILogger<CatalogService> log,
            IoTContext context)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            
        }

        /// <summary>
        /// Queryable Utilizado por ODATA
        /// </summary>
        public IQueryable<CatalogItemsInfo> CatalogItemsInfo
        {
            get {
                _log.LogInformation("llamando al catalogo");
                return _context.CatalogItemsInfos; 
            
            }
        }

    }
}
