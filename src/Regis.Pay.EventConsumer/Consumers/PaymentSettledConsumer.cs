using MassTransit;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentSettledConsumer : IConsumer<PaymentSettled>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentSettledConsumer> _logger;

        public PaymentSettledConsumer(
            IPaymentRepository paymentRepository,
            ILogger<PaymentSettledConsumer> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentSettled> context)
        {
            _logger.LogInformation("Consuming {event} for paymentId: {paymentId}", nameof(PaymentSettled), context.Message.AggregateId);

            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            await Task.Delay(300); // Do some process here on payment settled. eg. send out webhook.

            payment.Complete();

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
