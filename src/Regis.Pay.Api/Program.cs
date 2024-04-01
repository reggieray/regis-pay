using FastEndpoints;
using Regis.Pay.Common;
using Regis.Pay.Domain;
using Common = Regis.Pay.Common.ServiceCollectionExtensions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddFastEndpoints();
        builder.Services.AddEventStore(builder.Configuration);
        builder.Services.AddDomain();

        await Common.InitializeCosmos(builder.Services, builder.Configuration);

        var app = builder.Build();

        app.MapGet("/", () => "Hello Regis.Pay.Api!");

        app.UseFastEndpoints();
        app.Run();
    }
}