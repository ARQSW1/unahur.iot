namespace UNAHUR.IoT.Messaging.Contracts
{
    /// <summary>
    /// Interface para enviar un mensaje generico
    /// </summary>
    public interface IDummyMessage
    {
        /// <summary>
        /// Contenido del mensaje
        /// </summary>
        public string Message { get; set; }
    }
}