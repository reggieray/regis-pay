using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.MultiEventTestConsumer
{
    public class MultiPaymentCompletedEventTestConsumer : MultiEventTestConsumerBase<PaymentCompleted>
    {
        public override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentCompleted";

        public override void Add(PaymentCompleted @event)
        {
            EventIds.Add(@event.AggregateId);
        }
    }
}
