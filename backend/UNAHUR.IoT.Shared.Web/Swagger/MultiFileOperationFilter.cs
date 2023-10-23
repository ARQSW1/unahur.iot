using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace UNAHUR.IoT.Shared.Web.Swagger
{
    /// <summary>
    /// Filtro de operaciones API que suben mas de un archivo 
    /// </summary>
    /// <remarks>
    /// Las operaciones que suben mas de un archivo se detectan por que poseeun parametro del tipo <see cref="IFormFileCollection"/>
    /// </remarks>
    public class MultiFileOperationFilter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodParams = context.MethodInfo.GetParameters();
            var isFileUploadOperation = methodParams.Any(p => p.ParameterType.FullName.Equals(typeof(IFormFileCollection).FullName));

            if (!isFileUploadOperation) return;

            var uploadFileMediaType = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "object",
                    Properties =
                    {
                        ["uploadedFiles"] = new OpenApiSchema()
                        {
                            Description = "Upload Files",
                            Type = "array",
                            Items= new OpenApiSchema()
                            {
                                Type = "string", 
                                Format = "binary",
                            },
                        }
                    },
                    Required = new HashSet<string>()
                    {
                        "uploadedFiles"
                    }
                }
            };
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = uploadFileMediaType
                }
            };
        }
    }
}