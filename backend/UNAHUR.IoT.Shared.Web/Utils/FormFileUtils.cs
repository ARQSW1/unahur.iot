using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UNAHUR.IoT.Shared.Web.Utils
{
    public static class FormFileUtils
    {
        /// <summary>
        /// Convierte  <paramref name="files"/> en un <see cref="DataTable"/>
        /// </summary>
        /// <param name="files">Diccionario con los archivos</param>
        /// <param name="encodingName">Nombre del encoddig para validar lso archivos</param>
        /// <param name="fileNameColumn">NOmbre de la columna con el nombre del archivo</param>
        /// <param name="fileDataColum">Columna con los datos del archivo</param>
        /// <returns><see cref="DataTable"/> con dos campos <paramref name="fileNameColumn"/> con el nombre del arhcivo en <see cref="string"/> y <paramref name="fileDataColum"/> con lso datos del archivo en formato <see cref="byte[]"/></returns>
        public static async Task<DataTable> GetDataTableFromFilesAsync(IFormFileCollection files, string encodingName = "UTF-8", string fileNameColumn = "FileName", string fileDataColum = "FileData", CancellationToken cancellationToken = default)
        {
            var encoding = Encoding.GetEncoding(encodingName, new EncoderExceptionFallback(), new DecoderExceptionFallback());
            var validations = new List<FluentValidation.Results.ValidationFailure>();
            var dt = new DataTable();

            // genera el datatable con el esquema indicado
            dt.Columns.Add(fileNameColumn, typeof(string));
            dt.Columns.Add(fileDataColum, typeof(byte[]));


            for (int i = 0; i < files.Count; i++)
            {
                // abre el archivo como stream
                using (Stream fileStream = files[i].OpenReadStream())
                {
                    // copia el contenido a memoria para poder grabarlo, consume mas memoria pero ahorra procesador y complejidad
                    using var memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream, cancellationToken);
                    try
                    {
                        var data = memoryStream.ToArray();
                        // verifica el encodding (UTF-8) 256 bytes
                        var strData = encoding.GetString(data, 0, 256);
                        dt.Rows.Add(files[i].FileName, data);
                    }
                    catch (DecoderFallbackException ex)
                    {
                        // falla del encodding
                        validations.Add(new FluentValidation.Results.ValidationFailure(files[i].FileName, $"UTF-8 decodig error: {ex.Message}"));
                    }
                    catch (Exception ex)
                    {
                        // otro tipo de falla
                        validations.Add(new FluentValidation.Results.ValidationFailure(files[i].FileName, $"Error processing file: {ex.Message}"));
                    }
                }
            }
            // arroja todos los errores de los archivos juntos
            if (validations.Count > 0)
                throw new FluentValidation.ValidationException("Errors detected processing files", validations);


            return dt;
        }
    }
}
