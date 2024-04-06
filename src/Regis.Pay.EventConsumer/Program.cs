using Regis.Pay.Common;
using Regis.Pay.Domain;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEventStore(builder.Configuration);
        builder.Services.AddMessagingBus(builder.Configuration, addConsumers: true);
        builder.Services.AddDomain();
        builder.Services.AddCosmosDb(builder.Configuration);

        var app = builder.Build();

        app.MapGet("/", () => "Hello Regis.Pay.EventConsumer!");

        await app.RunAsync();
    }
}