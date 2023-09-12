using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UNAHUR.IoT.Api
{
    // ver https://github.com/dotnet/aspnet-api-versioning/wiki/Swashbuckle-Integration
    /// <summary>
    /// Opciones de configuracion de swagger
    /// </summary>
    internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = "UNAHUR.IoT API",
                        Description = "UNAHUR.IoT external API",
                        Version = description.ApiVersion.ToString(),
                    });
            }
            // inserta la documentacion xml en la documentacion swagger
            var xmlDocFiles = new List<string>
                {
                    Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"),
                    // esto debe estar aca?
                    // agrega el documento con el DAL asi los tipos de datos que vienen del context tiene documentacion
                    Path.Combine(AppContext.BaseDirectory, $"{typeof(DAL.Context.IoTContext).Assembly.GetName().Name}.xml"),
                };

            foreach (var file in xmlDocFiles)
            {
                if (File.Exists(file))
                    // inserta la documentacion en swagger gen
                    options.IncludeXmlComments(file, true);
            }

            // para evitar conflicto cuando hay una misma api en dos versiones
            //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.CustomSchemaIds(type => type.FullName);
        }
    }
}
