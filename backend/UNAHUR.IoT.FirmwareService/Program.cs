namespace UNAHUR.IoT.FirmwareService
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            // CREAR EL APP BUILDER,ES EL ENCARGADO DE CONFIGURAR LA INYECCION DE DEPENDENCIAS
            // Y LOS ORIGENES DE CONFIGURACION
            var builder = WebApplication.CreateBuilder(args);

            #region LOGGING 
            // CONFIGURA EL DESTINO DE LOG EN CASO QUE FALLE LA CONFIGURACION DE SERILOG
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            // logging desde la configuracion (CONVIENE HACERLO PRIMERO DE TODO ASI LOGUEA DESDE EL INICIO)
            builder.Host.UseSerilog((ctx, logConfig) =>
            {
                logConfig.ReadFrom.Configuration(ctx.Configuration);

            });

            #endregion //LOGGING

            // REGISTRO DE SERVICIOS DE LA APLICACION
            builder.RegisterServices();

            WebApplication app = builder.Build();

            // MIDDLEWARE DEL ASP.NET PIPELINE
            app.SetupMiddlewarePipeline();

            // escucha los eventos de la aplicacion
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(() => Log.Information("Service started"));
            lifetime.ApplicationStopping.Register(() => Log.Information("Service stoped"));

            app.Run();
        }
    }
}