using Regis.Pay.ChangeFeed;
using Regis.Pay.Common;
using Regis.Pay.Domain;
using Common = Regis.Pay.Common.ServiceCollectionExtensions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEventStore(builder.Configuration);
        builder.Services.AddMessagingBus(builder.Configuration);
        builder.Services.AddDomain();

        builder.Services.AddSingleton<IChangeEventHandler, ChangeEventHandler>();
        
        builder.Services.AddHostedService<Worker>();

        await Common.InitializeCosmos(builder.Services, builder.Configuration);

        var app = builder.Build();
        
        app.MapGet("/", () => "Hello Regis.Pay.ChangeFeed!");

        app.Run();
    }
}