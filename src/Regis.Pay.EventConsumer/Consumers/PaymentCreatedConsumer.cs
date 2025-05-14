using MassTransit;
using Regis.Pay.Common.ApiClients.Payments;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentCreatedConsumer : IConsumer<PaymentCreated>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentCreatedConsumer> _logger;
        private readonly IPaymentsApi _paymentsApi;

        public PaymentCreatedConsumer(
            IPaymentRepository paymentRepository,
            ILogger<PaymentCreatedConsumer> logger,
            IPaymentsApi paymentsApi)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _paymentsApi = paymentsApi;
        }

        public async Task Consume(ConsumeContext<PaymentCreated> context)
        {
            _logger.LogInformation("Consuming {event} for paymentId: {paymentId}", nameof(PaymentCreated), context.Message.AggregateId);

            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            var resonse = await _paymentsApi.SettlePaymentAsync(payment.ThridPartyReference!.Value);

            resonse.EnsureSuccessStatusCode();

            payment.Settled();

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
