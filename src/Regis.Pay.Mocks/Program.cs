using Regis.Pay.Mocks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSingleton<IInMemoryMockingService, InMemoryMockingService>();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("psp/api/payments/create", (IInMemoryMockingService mockingService, CreatePaymentRequest request) =>
{
    if (mockingService.ShouldError())
    {
        return Results.StatusCode(500);
    }

    return Results.Accepted(value: new CreatePaymentResponse(Guid.NewGuid()));
});

app.MapPost("psp/api/payments/{paymentId:guid}/settle", (IInMemoryMockingService mockingService) =>
{
    if (mockingService.ShouldError())
    {
        return Results.StatusCode(500);
    }

    return Results.Ok();
});

app.MapPost("notifications/api/send", (IInMemoryMockingService mockingService, NotificationRequest request) =>
{
    if (mockingService.ShouldError())
    {
        return Results.StatusCode(500);
    }

    return Results.Ok();
});

app.MapPost("toggle-errors", (IInMemoryMockingService mockingService) =>
{
    if (mockingService.ShouldError())
    {
        mockingService.TurnErrorOff();
    }
    else 
    {
        mockingService.TurnErrorOn();
    }

    return Results.Ok();
});

app.Run();

internal record CreatePaymentRequest(decimal Amount, string Currency);

internal record CreatePaymentResponse(Guid PaymentId);

internal record NotificationRequest(Guid Id, string JsonPayload);