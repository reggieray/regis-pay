using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Regis.Pay.Common.EventStore;

namespace Regis.Pay.Api.UnitTests
{
    class RegisPayApi : WebApplicationFactory<Program>
    {
        public readonly Mock<IEventStore> MockEventStore = new();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IEventStore));
                services.AddSingleton(MockEventStore.Object);
            });

            return base.CreateHost(builder);
        }
    }
}
