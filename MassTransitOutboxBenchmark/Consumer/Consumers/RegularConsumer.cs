using Contract;
using MassTransit;

namespace Consumer.Consumers
{
    public class RegularConsumer : IConsumer<RegularEvent>
    {
        private readonly ILogger<RegularConsumer> _logger;

        public RegularConsumer(ILogger<RegularConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RegularEvent> context)
        {
            _logger.LogDebug($"Received {nameof(RegularEvent)} - {context.Message.Name}");
            return Task.CompletedTask;
        }
    }
}
