using MassTransit.Serialization;
using Microsoft.Extensions.Options;
using Minio;
using System;
using System.Net;
using System.Threading.Tasks;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public class MinioStorage
    {
        private const string ENDPOINT = "ENDPOINT";
        private const string ACCESSKEY = "ACCESSKEY";
        private const string SECRETKEY = "SECRETKEY";
        private const string SECURE = "SECURE";
        private const string BUCKET = "BUCKET";
        private const string REGION = "REGION";

        private readonly StorageConfig _config;
        private readonly IMinioClient _minioClient;
        private readonly string _bucket;

        public MinioStorage(IOptions<StorageConfig> config)
        {

            _config = config.Value;

            if (!_config.Properties.ContainsKey(ENDPOINT))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{ENDPOINT} configuration");

            if (!_config.Properties.ContainsKey(ACCESSKEY))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{ACCESSKEY} configuration");

            if (!_config.Properties.ContainsKey(SECRETKEY))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{SECRETKEY} configuration");

            if (!_config.Properties.ContainsKey(BUCKET))
                throw new IndexOutOfRangeException($"Missing Storage.Properties.{BUCKET} configuration");

            _bucket = _config.Properties[BUCKET].ToString();

            var minioBuilder = new MinioClient()
                              .WithEndpoint(_config.Properties[ENDPOINT].ToString())
                              .WithCredentials(_config.Properties[ACCESSKEY].ToString(), _config.Properties[SECRETKEY].ToString());

            // OPTIONAL PARAMETER
            if (_config.Properties.ContainsKey(REGION))
                minioBuilder.WithRegion(_config.Properties[REGION].ToString());

            if (_config.Properties.ContainsKey(SECURE))
                minioBuilder.WithSSL((bool)_config.Properties[SECURE]);

            //BUILD CONNECTION
            _minioClient = minioBuilder.Build();

        }


        public Task<bool> TestConnection()
        {
            var exists = await _minioClient.BucketExistsAsync(new Minio.DataModel.Args.BucketExistsArgs());
            return Task.FromResult(true);
        }

    }
}
