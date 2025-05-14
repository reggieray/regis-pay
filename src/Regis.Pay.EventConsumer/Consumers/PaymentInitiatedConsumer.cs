using MassTransit;
using Regis.Pay.Common.ApiClients.Payments;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentInitiatedConsumer : IConsumer<PaymentInitiated>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentInitiatedConsumer> _logger;
        private readonly IPaymentsApi _paymentsApi;

        public PaymentInitiatedConsumer(
            IPaymentRepository paymentRepository,
            ILogger<PaymentInitiatedConsumer> logger,
            IPaymentsApi paymentsApi)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _paymentsApi = paymentsApi;
        }

        public async Task Consume(ConsumeContext<PaymentInitiated> context)
        {
            _logger.LogInformation("Consuming {event} for paymentId: {paymentId}", nameof(PaymentInitiated), context.Message.AggregateId);

            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            var reponse = await _paymentsApi.CreatePaymentAsync(new CreatePaymentRequest(payment.Amount, payment.Currency));

            await reponse.EnsureSuccessfulAsync();

            payment.Created(reponse.Content!.PaymentId);

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
