namespace Contract
{
    public class RegularEvent
    {
        public Guid CorrelationId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}