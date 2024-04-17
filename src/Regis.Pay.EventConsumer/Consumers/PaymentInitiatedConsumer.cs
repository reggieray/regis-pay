using MassTransit;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentInitiatedConsumer : IConsumer<PaymentInitiated>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentInitiatedConsumer> _logger;

        public PaymentInitiatedConsumer(
            IPaymentRepository paymentRepository,
            ILogger<PaymentInitiatedConsumer> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentInitiated> context)
        {
            _logger.LogInformation("Consuming {event} for paymentId: {paymentId}", nameof(PaymentInitiated), context.Message.AggregateId);

            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            await Task.Delay(300); // Do some process here on payment initiated. eg. save to SQL database or third party system.

            payment.Created();

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
