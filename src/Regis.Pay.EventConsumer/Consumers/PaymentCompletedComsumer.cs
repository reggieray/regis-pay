using MassTransit;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentCompletedComsumer : IConsumer<PaymentCompleted>
    {
        private readonly ILogger<PaymentCompletedComsumer> _logger;

        public PaymentCompletedComsumer(ILogger<PaymentCompletedComsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PaymentCompleted> context)
        {
            _logger.LogInformation("PaymentCompleted for {paymentId}", context.Message.AggregateId);

            return Task.CompletedTask;
        }
    }
}
