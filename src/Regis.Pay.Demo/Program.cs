using MudBlazor.Services;
using Regis.Pay.Common.Configuration;
using Regis.Pay.Demo;
using Regis.Pay.Demo.Components;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); 

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddHttpClient<RegisPayApiClient>(client => client.BaseAddress = new("https://regis-pay-api"));

var options = new CosmosConfigOptions();
builder.Configuration.GetSection(CosmosConfigOptions.Position).Bind(options);

builder.Services.AddSingleton(sp =>
{
    return new CosmosDbService(options.Endpoint, options.AuthKeyOrResourceToken, options.DatabaseName, options.ContainerName);
});


// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
