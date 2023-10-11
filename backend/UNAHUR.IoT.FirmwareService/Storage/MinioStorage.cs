using MassTransit;
using MassTransit.Caching.Internals;
using MassTransit.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public class MinioStorage : IFirmwareStorage
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
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation </param>
        /// <returns><code>true</code> si la conexion exs exitosa, <code>false</code> si la conexion no es exitosa</returns>
        /// <remarks>Se utiliza el log para registrar cualquier error en la conexion</remarks>
        public async Task<bool> TestAsync(CancellationToken cancellationToken = default)
        {
            bool ok = true;
            try
            {
                await InnerTestConnectionAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error al probar conexion con MINIO");
                ok = false;
            }

            return ok;
        }



        // Ver https://github.com/minio/minio-dotnet/blob/master/Minio.Examples/Cases/PutObject.cs

        /// <summary>
        /// Sube un archivo al repositorio <paramref name="repo"/> el archivo especificado en <paramref name="file"/>
        /// </summary>
        ///<param name="file">Archivo de firmware a subir</param>
        /// <param name="tags">Tags del archivo</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation </param>
        /// <exception cref="FluentValidation.ValidationException">En caso de algun problema con el nombre del repo o del archivo</exception>
        /// <returns>The object ID</returns>
        public async Task<string> UploadAsync(
            FirmwareModel file,
            IDictionary<string, string> tags = null,
            CancellationToken cancellationToken = default)
        {
            await InnerTestConnectionAsync(cancellationToken);
            /*var metaData = new Dictionary<string, string>
                (StringComparer.Ordinal) { { "Test-Metadata", "Test  Test" } };
            */


            // IMPORTANTE: DOCUMENTACION ACERCA DEL OBJECTNAME Y EL FILENAME 
            // https://min.io/docs/minio/windows/administration/object-management.html#id1

            var objectName = GetObjetcName(file.Repo, file.Name);




            // Upload a file to bucket.
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithStreamData(file.DataStream)
                .WithObjectSize(file.DataStream.Length)
                //.WithHeaders(metaData)
                .WithTagging(Tagging.GetBucketTags(tags))
                .WithContentType(file.ContentType);

            try
            {
                var response = await minio.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);
            }

            catch (InvalidObjectNameException ex)
            {
                // ESTA EXCEPCION SE TOMA COMO UN VALIDATION EXCEPTION
                throw new FluentValidation.ValidationException(ex.Message);

            }
            catch (InvalidObjectPrefixException ex)
            {
                // ESTA EXCEPCION SE TOMA COMO UN VALIDATION EXCEPTION

                throw new FluentValidation.ValidationException(ex.Message);
            }




            return objectName;
        }

        public async Task<FirmwareModel> DownloadAsync(
            string repo,
            string name,
            CancellationToken cancellationToken = default)
        {

            var outputStream = new MemoryStream();
            var objectName = GetObjetcName(repo, name);

            // IMPORTANTE EL HECHO DE COPIAR EL STREAM EN VEZ DE ASIGNARLO DIRECTAMENTE
            // ES UN PROBLEMA REGISTRADO DEL SDK DE MINIO
            var args = new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithCallbackStream((s, t) => s.CopyToAsync(outputStream, t));
            var ret = await minio.GetObjectAsync(args, cancellationToken: cancellationToken);

            return new FirmwareModel(repo, name, ret.ContentType, outputStream);
        }




        #region Private Methods

        private string GetObjetcName(string repo, string name)
        {

            return repo + "/" + name;
        }

        /// <summary>
        /// Intenta conectarse al storage y si no existe, crear el bucket configurado en <see cref="FirmwareStorageConfig.Properties"/>
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation </param>
        /// <exception cref="Minio.Exceptions.AuthorizationException">When access or secret key is invalid</exception>
        /// <exception cref="Minio.Exceptions.InvalidBucketNameException">When bucketName is invalid</exception>
        /// <exception cref="NotImplementedException">When object-lock or another extension is not implemented</exception>

        private async Task InnerTestConnectionAsync(CancellationToken cancellationToken = default)
        {

            var beArgs = new BucketExistsArgs().WithBucket(_bucket);

            bool found = await minio.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);

            if (!found)
            {
                _log.LogInformation("El {_bucket} no existe y sera creado", _bucket);

                var mbArgs = new MakeBucketArgs()
                    .WithObjectLock()
                    .WithBucket(_bucket);
                await minio.MakeBucketAsync(mbArgs, cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion

    }
}
