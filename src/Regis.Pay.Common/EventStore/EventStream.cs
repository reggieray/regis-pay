namespace Regis.Pay.Common.EventStore
{
    public class EventStream
    {
        private readonly List<IDomainEvent> _events;

        public EventStream(string id, int version, IEnumerable<IDomainEvent> events)
        {
            Id = id;
            Version = version;
            _events = events.ToList();
        }

        public string Id { get; private set; }

        public int Version { get; private set; }

        public IEnumerable<IDomainEvent> Events
        {
            get { return _events; }
        }
    }
}
