using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.MultiEventTestConsumer
{
    public class MultiPaymentInitiatedEventTestConsumer : MultiEventTestConsumerBase<PaymentInitiated>
    {
        public override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentInitiated";

        public override void Add(PaymentInitiated @event)
        {
            EventIds.Add(@event.AggregateId);
        }
    }
}
