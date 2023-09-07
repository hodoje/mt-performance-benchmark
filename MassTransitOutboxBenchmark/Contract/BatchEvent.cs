namespace Contract
{
    public class BatchEvent
    {
        public Guid CorrelationId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
