namespace Regis.Pay.Common.EventStore
{
    public interface IDomainEvent
    {
        DateTime Timestamp { get; }
    }
}
