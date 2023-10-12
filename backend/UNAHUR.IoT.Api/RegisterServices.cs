using Efunds.Shared.Web.Swagger;
using UNAHUR.IoT.Shared.Web.Utils;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UNAHUR.IoT.Messaging.Configuration;
using UNAHUR.IoT.Business;
using Efunds.Shared.Web.Extensions;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

namespace UNAHUR.IoT.Api
{
    /// <summary>
    /// <see cref="WebApplicationBuilder"/> extensions
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        private const string JWT_SECTION_NAME = "JwtAuthentication";

        /// <summary>
        /// Service registration for the entire app
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            // ******* Access the configuration *******
            var config = builder.Configuration;
            var services = builder.Services;

            // ver https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/PII
            IdentityModelEventSource.ShowPII = true;

            // kestrel config
            services.Configure<KestrelServerOptions>(config.GetSection("Kestrel"));
            services.Configure<RabbitMqSettings>(config.GetSection("RabbitMQ"));
            // carga la configuracion de JWT
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, config.GetSection(JWT_SECTION_NAME));
            // Configuracion del JWT para inyectar en swagger
            var jwtOptions = config.GetSection(JWT_SECTION_NAME).Get<JwtBearerOptions>();


            services.AddHealthChecks();

            services.AddIoTBusiness(config);
            //services.AddFileStorage(config);

            #region SEGURIDAD
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                // reutiliza la marca ValidateIssuer para evitar que se verifique la validex del certificado del IDP
                if (!options.TokenValidationParameters.ValidateIssuer)
                    options.BackchannelHttpHandler = new DevelopmentBackchannelHttpHandler();
            });
            #endregion SEGURIDAD

            #region VERSIONADO DE API 
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                // versionado por namespace
                //config.Conventions.Add(new VersionByNamespaceConvention());
            });
            /*
            builder.Services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                // esto es verdadero por que colocamos la version de la api en el controlador
                setup.SubstituteApiVersionInUrl = true;
            });*/

            #endregion VERSIONADO DE API 

            services.AddControllers()
                .AddJsonOptions(options =>
            {
                // arregla un problema con los enums en swagger 
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1269#issuecomment-586284629
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            }).AddOData(opt =>
            {
                opt.AddRouteComponents("odata/v1", SetupOData.GetEdmModel_V1());
                // habilita las funciones en ODATA 
                opt.Select().Filter().OrderBy().Expand();
                // si se debe hacer un cambio drastico al ODATA se puede hacer en una version 2 simultanea
                //opt.AddRouteComponents("odata/v2", SetupOData.GetEdmModel_V2());

            });



            if (config.IsSwaggerEnabled())
            {

                // necesario para configurar la api con la descripcion que corresponda y la documentacion
                builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

                services.AddSwaggerGenExtended(c =>
                {


                    var authUrl = StringHelpers.UrlCombine(jwtOptions.Authority, "/protocol/openid-connect/auth");
                    var tokenhUrl = StringHelpers.UrlCombine(jwtOptions.Authority, "/protocol/openid-connect/token");

                    // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {

                                AuthorizationUrl = new Uri(authUrl),
                                TokenUrl = new Uri(tokenhUrl),
                                Scopes = new Dictionary<string, string>
                            {
                                { "openid", "openid" },
                                { "profile", "perfil" }
                            }
                            }
                        }
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[] { "profile" }
                    }
                });

                });
            }

            #region MassTransit
            services.AddMassTransit(x =>
            {

                services.Configure<MassTransitHostOptions>(config.GetSection("MassTransit"));
                x.AddDelayedMessageScheduler();

                x.ConfigureMassTransit((context, cfg) =>
                {
                    cfg.AutoStart = true;
                    cfg.ConfigureEndpoints(context);
                });
            });
            #endregion MassTransit
            return builder;
        }



    }
}
