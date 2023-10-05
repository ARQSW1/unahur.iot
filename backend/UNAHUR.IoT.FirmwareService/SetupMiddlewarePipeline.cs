using UNAHUR.IoT.Shared.Web.ErrorHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Efunds.Shared.Web.Extensions;

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

                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    //agrega un endpoint por cada version
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
            }

            app.UseRouting();


            // global cors policy
            /*
            var origins = app.Configuration.CorsOrigins();
            app.UseCors(x =>
            {
                x.AllowAnyMethod()
                .AllowAnyHeader();


                if (origins != null || origins.Contains("*"))
                {
                    // permite todos los origenes
                    x.SetIsOriginAllowed(hostName => true);
                    x.AllowCredentials();
                }
                else
                {
                    // https://*.example.com
                    x.WithOrigins(origins).SetIsOriginAllowedToAllowWildcardSubdomains();
                    x.AllowCredentials();
                }
            });*/

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                { Predicate = r => false });
                endpoints.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("ready")
                });

            });
            return app;
        }
    }
}