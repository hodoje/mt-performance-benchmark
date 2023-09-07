using Contract;
using MassTransit;

namespace Consumer.Consumers
{
    public class BatchConsumer : IConsumer<Batch<BatchEvent>>
    {
        private readonly ILogger<BatchConsumer> _logger;

        public BatchConsumer(ILogger<BatchConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Batch<BatchEvent>> context)
        {
            var batch = context.Message;
            _logger.LogDebug($"Received {batch.Length} messages in batch - waited for {context.Message.Mode}");
            foreach (var item in batch)
            {
                _logger.LogDebug($"Received {item.Message.Name} from batch");
            }
            return Task.CompletedTask;
        }
    }

    public class BatchConsumerDefinition : ConsumerDefinition<BatchConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<BatchConsumer> consumerConfigurator)
        {
            endpointConfigurator.ConcurrentMessageLimit = 100;

            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(10)
                .SetTimeLimit(s: 1)
                .SetConcurrencyLimit(10));
        }
    }
}
