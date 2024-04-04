using Microsoft.Azure.Cosmos;
using Regis.Pay.Common.Configuration;
using Regis.Pay.Common.EventStore;

namespace Regis.Pay.ChangeFeed
{
    public class Worker(
        ILogger<Worker> logger,
        CosmosClient cosmosClient,
        CosmosConfigOptions cosmosConfigOptions,
        IChangeEventHandler changeEventHandler) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly CosmosClient _cosmosClient = cosmosClient;
        private readonly CosmosConfigOptions _cosmosConfigOptions = cosmosConfigOptions;
        private readonly IChangeEventHandler _changeEventHandler = changeEventHandler;
        private ChangeFeedProcessor _changeFeedProcessor;

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
            await _changeEventHandler.HandleAsync(events, cancellationToken);
        }
    }
}
