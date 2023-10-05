namespace UNAHUR.IoT.FirmwareService.Storage
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Minio;
    using System;


    /// <summary>
    /// Extensiones de <see cref="IServiceCollection"/> para la capa de negocio de IoT
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configura el storage para el firmware
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config">Puntero a la raiz de configuracion</param>
        /// <param name="configSection">Nombre de la seccion donde esta la configuracion</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddFirmwareStorageService(this IServiceCollection services, IConfiguration config, string configSection = nameof(FirmwareStorageConfig))
        {
            services.Configure<FirmwareStorageConfig>(config.GetSection(configSection));

            var serviceProvider = services.BuildServiceProvider();
            

            var options = serviceProvider
                   .GetService<IOptions<FirmwareStorageConfig>>()!.Value;
            

            switch (options.StorageClass )
            {
                case FirmwareStorageClasses.Mongo:
                    throw new NotSupportedException("Mongo Firmware Storage not supported... yet");
                case FirmwareStorageClasses.FileSystem:
                    throw new NotSupportedException("FileSystem Firmware Storage not supported... yet");
                case FirmwareStorageClasses.S3:

                    services.AddS3(options);
                    break;
                default:
                    throw new NotSupportedException("Storage class not supported");
            }

            

            return services;
        }
        /// <summary>
        /// COnfigura todo lo necesario para el minio/s3 
        /// </summary>
        /// <param name="services">Servicio</param>
        /// <param name="_config">Configuracion del storage</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private static IServiceCollection AddS3(this IServiceCollection services, FirmwareStorageConfig _config) {

            string _bucket;
            bool _secure=false;

            if (!_config.Properties.ContainsKey(MinioStorage.ENDPOINT))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{MinioStorage.ENDPOINT} configuration");

            if (!_config.Properties.ContainsKey(MinioStorage.ACCESSKEY))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{MinioStorage.ACCESSKEY} configuration");

            if (!_config.Properties.ContainsKey(MinioStorage.SECRETKEY))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{MinioStorage.SECRETKEY} configuration");

            if (_config.Properties.ContainsKey(MinioStorage.BUCKET))
            {
                _bucket = _config.Properties[MinioStorage.BUCKET].ToString();
            }
            else
            { 
                _bucket = "firmware-registry";
            }

            if (_config.Properties.ContainsKey(MinioStorage.SECURE))
                _secure = (bool)_config.Properties[MinioStorage.SECURE];

            services.AddMinio(minioBuilder => {
                minioBuilder = new MinioClient()
                                  .WithEndpoint(_config.Properties[MinioStorage.ENDPOINT].ToString())
                                  .WithCredentials(_config.Properties[MinioStorage.ACCESSKEY].ToString(), _config.Properties[MinioStorage.SECRETKEY].ToString())
                                  .WithSSL(_secure);

                // OPTIONAL PARAMETERS
                if (_config.Properties.ContainsKey(MinioStorage.REGION))
                    minioBuilder.WithRegion(_config.Properties[MinioStorage.REGION].ToString());
            });

            services.AddTransient<IFirmwareStorage, MinioStorage>();
            

            return services;
        }

    }
}