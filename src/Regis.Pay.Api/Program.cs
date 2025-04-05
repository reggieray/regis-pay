using FastEndpoints;
using Regis.Pay.Common;
using Regis.Pay.Domain;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddFastEndpoints();
        builder.Services.AddEventStore(builder.Configuration);
        builder.Services.AddDomain();
        builder.Services.AddCosmosDb(builder.Configuration);

        var app = builder.Build();

        app.MapDefaultEndpoints();

        app.MapGet("/", () => "Hello Regis.Pay.Api!");

        app.UseFastEndpoints();
        await app.RunAsync();
    }
}