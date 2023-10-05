using MassTransit;
using System.Collections.Generic;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public enum FirmwareStorageClasses
    {
        Mongo,
        S3,
        FileSystem
    }

    /// <summary>
    /// Clase donde se cargan los valores de configuracion para el FIrmware Sotrage 
    /// </summary>
    public class FirmwareStorageConfig
    {
        /// <summary>
        /// Clase que maneja el Storage 
        /// </summary>
        public FirmwareStorageClasses StorageClass { get; set; }

        /// <summary>
        /// Parametros de configuracion propios de la <see cref="StorageClass"/> configurada
        /// </summary>
        public Dictionary<string,object> Properties { get; set; }

        public FirmwareStorageConfig() { 
            Properties = new Dictionary<string,object>();
        }

        

    }
}
