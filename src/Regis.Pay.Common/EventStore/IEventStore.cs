namespace Regis.Pay.Common.EventStore
{
    public interface IEventStore
    {
        Task<EventStream> LoadStreamAsync(string streamId);

        Task<bool> AppendToStreamAsync(
            string streamId,
            int expectedVersion,
            IEnumerable<IDomainEvent> events);
    }
}
