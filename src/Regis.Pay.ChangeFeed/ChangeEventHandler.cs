using MassTransit;
using Regis.Pay.Common.EventStore;
using Regis.Pay.Domain;

namespace Regis.Pay.ChangeFeed
{
    public interface IChangeEventHandler
    {
        Task HandleAsync(IReadOnlyCollection<EventWrapper> events, CancellationToken cancellationToken);
    }

    public class ChangeEventHandler(
        IBus bus,
        ILogger<ChangeEventHandler> logger) : IChangeEventHandler
    {
        private readonly IBus _bus = bus;
        private readonly ILogger<ChangeEventHandler> _logger = logger;

        public async Task HandleAsync(IReadOnlyCollection<EventWrapper> events, CancellationToken cancellationToken)
        {
            foreach (var @event in events)
            {
                _logger.LogInformation("Detected change feed {event} for {eventId}", @event.EventType, @event.Id);

                var integrationEvent = IntegrationEventResolver.Resolve(@event);

                if (integrationEvent is null)
                {
                    _logger.LogInformation("No integration event found for event with {eventId}", @event.Id);
                    break;
                }

                await _bus.Publish(integrationEvent, cancellationToken);
            }

            _logger.LogInformation("Finished handling changes.");
        }
    }
}
