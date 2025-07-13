using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.EventTestConsumer
{
    public class PaymentCompletedEventTestConsumer : EventTestConsumerBase<PaymentCompleted>
    {
        protected override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentCompleted";
    }
}
