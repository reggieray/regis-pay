namespace Regis.Pay.Domain.IntegrationEvents
{
    public abstract class IntegrationEventBase : IIntegrationEvent
    {
        public string AggregateId { get; set; } = default!;
    }
}
