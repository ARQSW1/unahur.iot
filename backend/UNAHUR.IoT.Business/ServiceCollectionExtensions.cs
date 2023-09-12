using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using UNAHUR.IoT.Business.Services;
using UNAHUR.IoT.DAL.Context;

namespace UNAHUR.IoT.Business
{
    /// <summary>
    /// Extensiones de <see cref="IServiceCollection"/> para la capa de negocio de IoT
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configura la capa de negocio de IoT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="_config">Puntero a la raiz de configuracion</param>
        /// <param name="configSection">Nombre de la seccion donde esta la configuracion</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddIoTBusiness(this IServiceCollection services, IConfiguration _config, string configSection = nameof(IoTConfig))
        {
            services.Configure<IoTConfig>(_config.GetSection(configSection));

            // Contexto de base de datos
            services.AddDbContext<IoTContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<IoTConfig>>().Value;

                if (String.IsNullOrEmpty(config.ConnectionString))
                    throw new Exception($"No se ha definido la propiedad {configSection}__ConnectionString");

                options.UseSqlServer(config.ConnectionString, x => x.UseHierarchyId()).EnableDetailedErrors();
            });

            #region Services
            services.AddTransient<CatalogService>();
            
            #endregion Services






            return services;

        }
    }
}