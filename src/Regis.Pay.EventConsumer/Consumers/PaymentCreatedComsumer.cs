using MassTransit;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentCreatedComsumer : IConsumer<PaymentCreated>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentCreatedComsumer> _logger;

        public PaymentCreatedComsumer(
            IPaymentRepository paymentRepository,
            ILogger<PaymentCreatedComsumer> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCreated> context)
        {
            _logger.LogInformation("Consuming {event} for paymentId: {paymentId}", nameof(PaymentCreated), context.Message.AggregateId);

            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            await Task.Delay(300); // Do some process here on payment created. eg. process payment.

            var thirdPartyReference = Guid.NewGuid();

            payment.Settled(thirdPartyReference);

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
