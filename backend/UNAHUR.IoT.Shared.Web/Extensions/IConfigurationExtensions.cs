using Microsoft.Extensions.Configuration;
using System;

namespace Efunds.Shared.Web.Extensions
{
    /// <summary>
    /// Configuration extensions for API
    /// </summary>
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Are swagger endpoints enabled?
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool IsSwaggerEnabled(this IConfiguration config)
        {
            try
            {
                return config.GetSection("SwaggerEnabled").Get<bool>();
            }
            catch
            {

                return false;
            }
        }

        /// <summary>
        /// CORS Origins in config
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string[] CorsOrigins(this IConfiguration config)
        {
            return config.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>();
        }
    }
}
