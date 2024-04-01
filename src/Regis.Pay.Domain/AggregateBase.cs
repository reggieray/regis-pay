using Regis.Pay.Common.EventStore;

namespace Regis.Pay.Domain
{
    public abstract class AggregateBase
    {
        public int Version { get; private set; }

        public List<IDomainEvent> Changes { get; }

        protected AggregateBase() 
        {
            Changes = new List<IDomainEvent>();
        }   

        protected AggregateBase(IEnumerable<IDomainEvent> events)
        {
            Changes = new List<IDomainEvent>();

            foreach (var @event in events)
            {
                Mutate(@event);
                Version += 1;
            }
        }

        protected void Apply(IDomainEvent @event)
        {
            Changes.Add(@event);
            Mutate(@event);
        }

        private void Mutate(IDomainEvent @event)
        {
            ((dynamic)this).When((dynamic)@event);
        }
    }
}
