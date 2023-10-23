using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using UNAHUR.IoT.Shared.Web.Utils;

namespace UNAHUR.IoT.OpenApi;


internal static class Startup
{
    internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config, Action<SwaggerGenOptions> setupAction = null)
    {
        var settings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        var jwtOptions = config.GetSection(nameof(JwtBearerOptions)).Get<JwtBearerOptions>();

        if (settings == null) return services;

        if (settings.Enable)
        {
            
            // esto solo hace falta si se usan minimal apis
            // services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {

                // Predicado de inclusion a swagger
                c.DocInclusionPredicate((name, api) =>
                {
                    // excluye todo lo que empieza con odata
                    var include = !api.RelativePath.Contains("odata");
                    return include;

                });

                // TODO: agregar definicion de seguridad 


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
                                { "profile", "profile" }
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
           
        }

        return services;
    }

    internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        var settings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

        if (settings.Enable)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                
                var apiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                //agrega un endpoint por cada version
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName);
                }
            });

            
        }

        return app;
    }
}
