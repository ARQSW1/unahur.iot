namespace UNAHUR.IoT.Messaging.Configuration
{
    /// <summary>
    /// Configuracion de conexion de RabbitMQ
    /// </summary>
    public class RabbitMqSettings
    {
        /// <summary>
        /// Url del rabbit
        /// </summary>

        public string Host { get; set; }

        /// <summary>
        /// BUerto TCP de Rabbit
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// VIrtual host a utilizar
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// NOmbre de usuario
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        public RabbitMqSettings()
        {
            this.Host = "localhost";
            this.Port = 5672;
            this.VirtualHost = "/";
            this.Username = "guest";
            this.Password = "guest";

        }
    }
}
