using Contract;
using MassTransit;

namespace Producer
{
    public class Producer
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public Producer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task ProduceBatchEvents(int messageCount)
        {
            var publishes = new List<Task>();
            for (var i = 1; i < messageCount + 1; i++)
            {
                publishes.Add(_publishEndpoint.Publish(new BatchEvent
                {
                    CorrelationId = Guid.NewGuid(),
                    Name = $"Batch-Test-{i}"
                }));
            }
            await Task.WhenAll(publishes);
        }

        public async Task ProduceRegularEvents(int messageCount)
        {
            var publishes = new List<Task>();
            for (var i = 1; i < messageCount + 1; i++)
            {
                publishes.Add(_publishEndpoint.Publish(new RegularEvent
                {
                    CorrelationId = Guid.NewGuid(),
                    Name = $"Regular-Test-{i}"
                }));
            }
            await Task.WhenAll(publishes);
        }
    }
}
