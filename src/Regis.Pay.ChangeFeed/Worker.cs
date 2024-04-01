using MassTransit;
using Microsoft.Azure.Cosmos;
using Regis.Pay.Common.Configuration;
using Regis.Pay.Common.EventStore;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.ChangeFeed
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly CosmosConfigOptions _cosmosConfigOptions;
        private readonly IBus _bus;
        private ChangeFeedProcessor _changeFeedProcessor;

        public Worker(
            ILogger<Worker> logger,
            CosmosClient cosmosClient,
            CosmosConfigOptions cosmosConfigOptions,
            IBus bus)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
            _cosmosConfigOptions = cosmosConfigOptions;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _changeFeedProcessor = await StartChangeFeedProcessorAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _changeFeedProcessor.StopAsync();
        }

        private async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync()
        {
            var leaseContainer = _cosmosClient.GetContainer(_cosmosConfigOptions.DatabaseName, _cosmosConfigOptions.LeasesContainerName);
            ChangeFeedProcessor changeFeedProcessor = _cosmosClient.GetContainer(_cosmosConfigOptions.DatabaseName, _cosmosConfigOptions.ContainerName)
                .GetChangeFeedProcessorBuilder<EventWrapper>(processorName: "eventsChangeFeed", onChangesDelegate: HandleChangesAsync)
                    .WithInstanceName("Regis.Pay.ChangeFeed")
                    .WithLeaseContainer(leaseContainer)
                    .Build();

            _logger.LogInformation("Starting Change Feed Processor...");
            await changeFeedProcessor.StartAsync();
            _logger.LogInformation("Change Feed Processor started.");
            return changeFeedProcessor;
        }

        async Task HandleChangesAsync(
                ChangeFeedProcessorContext context,
                IReadOnlyCollection<EventWrapper> events,
                CancellationToken cancellationToken)
        {
            foreach (var @event in events)
            {
                _logger.LogInformation("Detected change feed {event} for {eventId}", @event.EventType, @event.Id);

                //TODO: refactor

                var integrationEvent = IntegrationEventResolver.Resolve(@event);

                if (integrationEvent is null)
                {
                    _logger.LogInformation("No integration event found for event with {eventId}", @event.Id);
                }
                else
                {
                    await PublishEvent(integrationEvent, @event.EventType);
                }
            }

            _logger.LogInformation("Finished handling changes.");
        }

        private async Task PublishEvent(IIntegrationEvent @event, string eventType)
        {
            if (eventType == nameof(PaymentInitiated))
            {
                await _bus.Publish<PaymentInitiated>(@event);
            }

            if (eventType == nameof(PaymentCreated))
            {
                await _bus.Publish<PaymentCreated>(@event);
            }

            if (eventType == nameof(PaymentSettled))
            {
                await _bus.Publish<PaymentSettled>(@event);
            }
        }
    }
}
