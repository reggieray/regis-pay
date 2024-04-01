using Regis.Pay.Common.EventStore;
using Regis.Pay.Domain.Events;

namespace Regis.Pay.Domain
{
    public static class IntegrationEventResolver
    {
        public static IIntegrationEvent? Resolve<TDomainEvent>(TDomainEvent @event) where TDomainEvent : EventWrapper
        {
            return @event.EventType switch
            {
                nameof(PaymentInitiated) => new IntegrationEvents.PaymentInitiated { AggregateId = @event.StreamInfo.Id },
                nameof(PaymentCreated) => new IntegrationEvents.PaymentCreated { AggregateId = @event.StreamInfo.Id },
                nameof(PaymentSettled) => new IntegrationEvents.PaymentSettled { AggregateId = @event.StreamInfo.Id },
                _ => default
            };
        }
    }
}
