using Microsoft.AspNetCore.Builder;

namespace UNAHUR.IoT.Shared.Web.ErrorHandling
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {

        /// <summary>
        /// Registers the <see cref="RFC7807ErrorHandlerMiddleware"/> to catch all unhanled exceptions
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRFC7807ErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RFC7807ErrorHandlerMiddleware>();
        }
    }
}
