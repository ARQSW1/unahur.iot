using UNAHUR.IoT.Shared.Web.ErrorHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UNAHUR.IoT.Shared.Web.Extensions;
using Asp.Versioning.ApiExplorer;
using System.Linq;
using Minio.Exceptions;

namespace UNAHUR.IoT.FirmwareService
{
    /// <summary>
    ///  Clase de extension de <see cref="WebApplication"/> 
    /// </summary>
    public static class SetupExtensions
    {


        /// <summary>
        /// Register all the midleware pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication SetupMiddlewarePipeline(this WebApplication app)
        {
            // serilog va primero por que sino no detecta los cambios hechos por el UseRFC7807ErrorHandler
            app.UseSerilogRequestLogging();
            
            // MUY IMPORTANTE A TENER EN CUENTA
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0#middleware-order
            app.UseRFC7807ErrorHandler();

            // CUando el servicio corre detras de uno o varios proxys se pierde la direccion IP del cliente. 
            // Para ello el o los proxys agregan la direccion original en un HTTP Header
            // Esta opcion captura la direccion IP del cliente de ese header
            // INDISPENSABLE PARA AUDITORIA
            app.UseForwardedHeaders();


            if (app.Configuration.IsSwaggerEnabled())
            {
                // el cors en .net core no lo pude hacer funcionar
                app.Use((context, next) =>
                {
                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    return next.Invoke();
                });

                app.UseSwagger();
            }

            app.UseRouting();
            
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/healthz/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            { Predicate = r => false });

            app.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("ready")
            });
            
            return app;
        }
    }
}