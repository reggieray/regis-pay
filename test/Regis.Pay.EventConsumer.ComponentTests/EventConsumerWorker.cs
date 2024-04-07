using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Regis.Pay.Common.EventStore;

namespace Regis.Pay.EventConsumer.ComponentTests
{
    class EventConsumerWorker : WebApplicationFactory<Program>
    {
        public readonly Mock<IEventStore> MockEventStore = new();
        public readonly Mock<CosmosClient> MockCosmosClient = new();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IEventStore));
                services.AddSingleton(MockEventStore.Object);

                services.RemoveAll(typeof(CosmosClient));
                services.AddSingleton(MockCosmosClient.Object);

                services.AddMassTransitTestHarness();
            });

            return base.CreateHost(builder);
        }
    }
}
