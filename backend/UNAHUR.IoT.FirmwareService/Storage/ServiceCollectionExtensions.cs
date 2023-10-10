namespace UNAHUR.IoT.FirmwareService.Storage
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
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
            string _endpoint;
            string _accessKey;
            string _secretKey;
            bool _secure=false;


            if (!_config.Properties.ContainsKey(MinioStorage.ENDPOINT))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{MinioStorage.ENDPOINT} configuration");
            else
                _endpoint = _config.Properties[MinioStorage.ENDPOINT].ToString();

            if (!_config.Properties.ContainsKey(MinioStorage.ACCESSKEY))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{MinioStorage.ACCESSKEY} configuration");
            else
                _accessKey = _config.Properties[MinioStorage.ACCESSKEY].ToString();

            if (!_config.Properties.ContainsKey(MinioStorage.SECRETKEY))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{MinioStorage.SECRETKEY} configuration");
            else
                _secretKey = _config.Properties[MinioStorage.SECRETKEY].ToString();

            if (_config.Properties.ContainsKey(MinioStorage.BUCKET))
            {
                _bucket = _config.Properties[MinioStorage.BUCKET].ToString();
            }
            else
            { 
                _bucket = "firmware-registry";
            }

            if (_config.Properties.ContainsKey(MinioStorage.SECURE))
                bool.TryParse(_config.Properties[MinioStorage.SECURE].ToString(),out _secure);

            // Este AddMinio BuiltIn no funciona , no toma el endpoint

            /* services.AddMinio(configureClient => configureClient
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey,_secretKey)
                .WithSSL(_secure));
            */

            services.TryAddTransient(_ => new MinioClient().WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
                .WithSSL(_secure).Build());


            services.AddTransient<IFirmwareStorage, MinioStorage>();
            

            return services;
        }

    }
}