namespace UNAHUR.IoT.Messaging.Contracts
{
    /// <summary>
    /// Interface para enviar un mensaje generico
    /// </summary>
    public interface INewDeviceMessage
    {
        /// <summary>
        /// Identificador unico del dispositivo
        /// </summary>
        public string DeviceId { get; set; }
    }
}