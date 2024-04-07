 namespace Regis.Pay.EndToEndTests.EventTestConsumer
{
    public class PaymentCompletedEventTestConsumer : EventTestConsumerBase<PaymentCompleted>
    {
        public override string ExchangeName => "Regis.Pay.Domain.IntegrationEvents:PaymentCompleted";
    }

    public class PaymentCompleted
    {
        public string AggregateId { get; set; } = default!;
    }
}
