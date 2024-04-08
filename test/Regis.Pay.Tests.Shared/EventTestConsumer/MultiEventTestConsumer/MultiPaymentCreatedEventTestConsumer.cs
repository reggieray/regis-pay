using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.MultiEventTestConsumer
{
    public class MultiPaymentCreatedEventTestConsumer : MultiEventTestConsumerBase<PaymentCreated>
    {
        public override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentCreated";

        public override void Add(PaymentCreated @event)
        {
            EventIds.Add(@event.AggregateId);
        }
    }
}
