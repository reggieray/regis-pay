using Regis.Pay.Common.EventStore;

namespace Regis.Pay.Domain.Events
{
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
