﻿using MassTransit;
using MassTransit.Caching.Internals;
using MassTransit.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public class MinioStorage: IFirmwareStorage
    {
        // CONSTANTES DE CONFIGURACION
        internal const string ENDPOINT = "ENDPOINT";
        internal const string ACCESSKEY = "ACCESSKEY";
        internal const string SECRETKEY = "SECRETKEY";
        internal const string SECURE = "SECURE";
        internal const string BUCKET = "BUCKET";
        internal const string REGION = "REGION";

        private readonly IMinioClient minio;
        private readonly ILogger _log;
        private readonly string _bucket;
        private readonly FirmwareStorageConfig _config;

        public MinioStorage(ILogger<MinioStorage> log, IOptions<FirmwareStorageConfig> config, IMinioClient minio)
        {
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));

            _log = log ?? throw new ArgumentNullException(nameof(log));

            if (_config.Properties.ContainsKey(BUCKET))
            {
                _bucket = _config.Properties[BUCKET].ToString();
            }
            else
            {
                _log.LogWarning($"Missing Storage.Properties.{BUCKET} configuration defaulting to 'firmware-storage' bucket");
                _bucket = "firmware-storage";
            }

            this.minio = minio ?? throw new ArgumentNullException(nameof(minio));
        }


        /// <summary>
        /// Intenta conectarse al storage
        /// </summary>
        /// <returns><code>true</code> si la conexion exs exitosa, <code>false</code> si la conexion no es exitosa</returns>
        /// <remarks>Se utiliza el log para registrar cualquier error en la conexion</remarks>
        public async Task<bool> TestConnection()
        {
            bool found = false;
            try
            {
                found = await minio.BucketExistsAsync(new Minio.DataModel.Args.BucketExistsArgs().WithBucket(_bucket));
                if (!found)
                    _log.LogError("El bucket {_bucket} no existe", _bucket);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error al probar conexion con MINIO");
            }

            return found;
        }

        // Ver https://github.com/minio/minio-dotnet/blob/master/Minio.Examples/Cases/PutObject.cs
        public async Task<string> UploadAsync(string repo, string tag, IFormFile file, CancellationToken cancelationToken= default){

            // Make a bucket on the server, if not already present.
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucket);
            bool found = await minio.BucketExistsAsync(beArgs, cancelationToken).ConfigureAwait(false);
            
            if (!found)
            {
                _log.LogInformation("El {_bucket} no existe y sera creado",_bucket);

                var mbArgs = new MakeBucketArgs()
                    .WithBucket(_bucket);
                await minio.MakeBucketAsync(mbArgs, cancelationToken).ConfigureAwait(false);
            }


            var metaData = new Dictionary<string, string>
                (StringComparer.Ordinal) { { "Test-Metadata", "Test  Test" } };

            // check if object exists
            // IMPORTANTE: DOCUMENTACION ACERCA DEL OBJECTNAME Y EL FILENAME 
            // https://min.io/docs/minio/windows/administration/object-management.html#id1
            var objectName = repo + "/" + tag;

            var fileName = file.Name;

            using (var stream = file.OpenReadStream())
            {
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(objectName)
                    .WithFileName(fileName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithHeaders(metaData)
                    .WithContentType("application/octet-stream");

                var response = await minio.PutObjectAsync(putObjectArgs, cancelationToken).ConfigureAwait(false);
            }
            // response.Etag

            return objectName;
        }

    }
}