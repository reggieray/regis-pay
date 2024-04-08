using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.MultiEventTestConsumer
{
    public class MultiPaymentSettledEventTestConsumer : MultiEventTestConsumerBase<PaymentSettled>
    {
        public override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentSettled";

        public override void Add(PaymentSettled @event)
        {
            EventIds.Add(@event.AggregateId);
        }
    }
}
