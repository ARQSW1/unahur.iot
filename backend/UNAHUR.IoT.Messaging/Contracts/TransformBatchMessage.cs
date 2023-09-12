namespace UNAHUR.IoT.Messaging.Contracts
{
    public record TransformBatchMessage
    {
        public Guid BatchId { get; set; }
        public string? User { get; set; }
    }
}
