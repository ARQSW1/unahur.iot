using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNAHUR.IoT.DAL.Context;
using UNAHUR.IoT.DAL.Models;

namespace UNAHUR.IoT.Business.Services
{
    /// <summary>
    /// Encapsula la funcionalidad de los firmware
    /// </summary>
    internal class FirmwareService
    {
        private readonly ILogger _log;
        private readonly IoTContext _context;

        /// <summary>
        /// Crea una instancia del servicio
        /// </summary>
        /// <param name="log"></param>
        /// <param name="context"></param>
        public FirmwareService(
            ILogger<FirmwareService> log,
            IoTContext context)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        // TODO: UPLOAD
        // TODO: DOWNLOAD
        //TODO: QUERYABLES 

        

    }
}
