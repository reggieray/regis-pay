using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Regis.Pay.EndToEndTests;

public class RegisPayFixture : IAsyncLifetime
{
    private DistributedApplication App { get; set; } = null!;

    public HttpClient? ApiClient { get; private set; }

    public string? RabbitMqConnString { get; private set; }
    
    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Regis_Pay_AppHost>();
        
        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        
        App = await builder.BuildAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        await App.ResourceNotifications.WaitForResourceHealthyAsync("regis-pay-eventconsumer", cts.Token);
        await App.ResourceNotifications.WaitForResourceHealthyAsync("regis-pay-api", cts.Token);
        await App.ResourceNotifications.WaitForResourceHealthyAsync("regis-pay-changefeed", cts.Token);
        await App.ResourceNotifications.WaitForResourceHealthyAsync("regis-pay-messaging", cts.Token);
        
        await App.StartAsync(cts.Token);
        
        ApiClient = App.CreateHttpClient("regis-pay-api");
        
        RabbitMqConnString = await App.GetConnectionStringAsync("regis-pay-messaging", cancellationToken: cts.Token);
    }

    public async Task DisposeAsync()
    {
        await App.StopAsync();
        await App.DisposeAsync();
    }
}