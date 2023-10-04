using MassTransit;
using System.Collections.Generic;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public class StorageConfig
    {
        /// <summary>
        /// Clase que maneja el Storage 
        /// </summary>
        public string StorageClass { get; set; }

        public Dictionary<string,object> Properties { get; set; }

        public StorageConfig() { 
            Properties = new Dictionary<string,object>();
        }

        

    }
}
