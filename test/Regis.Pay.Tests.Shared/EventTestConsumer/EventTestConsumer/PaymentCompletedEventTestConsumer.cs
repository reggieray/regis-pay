using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.EventTestConsumer
{
    public class PaymentCompletedEventTestConsumer : EventTestConsumerBase<PaymentCompleted>
    {
        public override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentCompleted";
    }
}
