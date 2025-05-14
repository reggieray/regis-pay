using MassTransit;
using Regis.Pay.Common.ApiClients.Notifications;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;
using System.Text.Json;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentSettledConsumer : IConsumer<PaymentSettled>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentSettledConsumer> _logger;
        private readonly INotificationsApi _notificationsApi;

        public PaymentSettledConsumer(
            IPaymentRepository paymentRepository,
            ILogger<PaymentSettledConsumer> logger,
            INotificationsApi notificationsApi)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _notificationsApi = notificationsApi;
        }

        public async Task Consume(ConsumeContext<PaymentSettled> context)
        {
            _logger.LogInformation("Consuming {event} for paymentId: {paymentId}", nameof(PaymentSettled), context.Message.AggregateId);

            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            var response = await _notificationsApi.SendNotificationAsync(new NotificationRequest(payment.PaymentId, JsonSerializer.Serialize(payment)));

            response.EnsureSuccessStatusCode();

            payment.Complete();

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
