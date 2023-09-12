using System.Net.Http;

namespace UNAHUR.IoT.Shared.Web.Utils
{
    /// <summary>
    /// Deshabilita la validacion de certificados HTTPS
    /// </summary>
    public class DevelopmentBackchannelHttpHandler : HttpClientHandler
    {
        public DevelopmentBackchannelHttpHandler() : base()
        {
            this.ServerCertificateCustomValidationCallback = delegate { return true; };
        }
    }
}
