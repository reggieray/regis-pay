using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Regis.Pay.Common.ApiClients.Payments;
using Regis.Pay.Common.EventStore;

namespace Regis.Pay.EventConsumer.ComponentTests;

internal class EventConsumerWorker : WebApplicationFactory<Program>
{
    public readonly Mock<IEventStore> MockEventStore = new();
    public readonly Mock<IPaymentsApi> MockPaymentsApi = new();
    private readonly Mock<CosmosClient> _mockCosmosClient = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IPaymentsApi));
            services.AddSingleton(MockPaymentsApi.Object);

            services.RemoveAll(typeof(IEventStore));
            services.AddSingleton(MockEventStore.Object);

            services.RemoveAll(typeof(CosmosClient));
            services.AddSingleton(_mockCosmosClient.Object);

            services.AddMassTransitTestHarness();
        });

        return base.CreateHost(builder);
    }
}