using UNAHUR.IoT.Shared.Web.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace UNAHUR.IoT.Shared.Web.Swagger
{
    /// <summary>
    ///
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configura swagger gen
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appName"></param>
        /// <returns></returns>

        public static IServiceCollection AddSwaggerGenExtended(this IServiceCollection services, Action<SwaggerGenOptions> setupAction = null)
        {
            var serviceProvider = services.BuildServiceProvider();

            services.AddSwaggerGen(c =>
            {

                // Predicado de inclusion a swagger
                c.DocInclusionPredicate((name, api) =>
                {
                    // excluye todo lo que empieza con odata
                    var include = !api.RelativePath.Contains("odata");
                    return include;

                });


                // https://github.com/dotnet/aspnet-api-versioning/wiki/Swashbuckle-Integration
                c.OperationFilter<SwaggerDefaultValues>();


                c.OperationFilter<StreamJsonContentFilter>();


                // TODO: REVISAR SI HACE FALTA
                c.OperationFilter<XOperationNameOperationFilter>();


                // Assign scope requirements to operations based on AuthorizeAttribute
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // filtro para que el swagger genere bien los uploads de archivo
                c.OperationFilter<SingleFileOperationFilter>();

                // filtro para que el swagger genere bien los uploads de multiples archivos
                c.OperationFilter<MultiFileOperationFilter>();

                c.DescribeAllParametersInCamelCase();

                setupAction?.Invoke(c);


            });

            return services;

        }
    }
}