﻿namespace UNAHUR.IoT;

using Asp.Versioning;
using UNAHUR.IoT.Cors;
using UNAHUR.IoT.Middleware;
using UNAHUR.IoT.OpenApi;
using UNAHUR.IoT.SecurityHeaders;
using UNAHUR.IoT.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Linq;

public static class Startup
{
    


    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // kestrel config
        services.Configure<KestrelServerOptions>(config.GetSection(nameof(KestrelServerOptions)));

        // esto lo ponemos aca?
        //services.Configure<RabbitMqSettings>(config.GetSection("RabbitMQ"));

        // carga la configuracion de JWT
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, config.GetSection(nameof(JwtBearerOptions)));
        

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        return services
           .AddApiVersioning()
           .AddAuth()
           .AddCorsPolicy(config)
           .AddHealthCheck()
           .AddOpenApiDocumentation(config)
           .AddRouting(options => options.LowercaseUrls = true);
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        }).Services;

    private static IServiceCollection AddAuth(this IServiceCollection services) =>
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            // reutiliza la marca ValidateIssuer para evitar que se verifique la validex del certificado del IDP
            if (!options.TokenValidationParameters.ValidateIssuer)
                options.BackchannelHttpHandler = new DevelopmentBackchannelHttpHandler();
        }).Services;


    private static IServiceCollection AddHealthCheck(this IServiceCollection services) =>
        services.AddHealthChecks().Services;

    /// <summary>
    /// Adds 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config)
    {
        builder
            .UseSerilogRequestLogging()
            .UseForwardedHeaders()
            .UseSecurityHeaders(config)
            .UseExceptionMiddleware()
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            .UseAuthorization()
            .UseOpenApiDocumentation(config);

        return builder;
    }
    /// <summary>
    /// Mapea los endpoints de los controladores y los HealthChecks
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers().RequireAuthorization();

        // TODO: REVISAR BIEN
        builder.MapHealthChecks("/healthz/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        { Predicate = r => false });

        // TIENEN QUE ESTA TAGUEADOS CON "ready"
        builder.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Any(t=> t.ToLowerInvariant().Equals("ready"))
        });

        return builder;
    }


}

