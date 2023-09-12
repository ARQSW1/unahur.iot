using UNAHUR.IoT.Shared.Web.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Efunds.Shared.Web.Swagger
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

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


                // definie el operation id default
                /*c.CustomOperationIds(c =>
                {

                    if (c.TryGetMethodInfo(out MethodInfo methodInfo))
                    {
                        return methodInfo.Name + methodInfo.;
                    }
                    else
                    {
                        return null;
                    }


                });*/


                c.OperationFilter<SwaggerDefaultValues>();

                c.OperationFilter<StreamJsonContentFilter>();

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