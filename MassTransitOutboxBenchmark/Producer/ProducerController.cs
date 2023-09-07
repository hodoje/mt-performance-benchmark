using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Producer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly ILogger<ProducerController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ProducerContext _context;

        public ProducerController(ILogger<ProducerController> logger, IPublishEndpoint publishEndpoint, ProducerContext context)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _context = context;
        }

        [HttpPost]
        [Route("produce-regular-events")]
        public async Task<IActionResult> ProduceRegularEvents(int messageCount)
        {
            var producer = new Producer(_publishEndpoint);
            await producer.ProduceRegularEvents(messageCount);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"Produced {messageCount} messages");
            return Ok();
        }

        [HttpPut]
        [Route("produce-batch-events")]
        public async Task<IActionResult> ProduceBatchEvents(int messageCount)
        {
            var producer = new Producer(_publishEndpoint);
            await producer.ProduceBatchEvents(messageCount);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"Produced {messageCount} messages");
            return Ok();
        }
    }
}
