var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("psp/api/payments/create", (CreatePaymentRequest request) =>
{
    return Results.Accepted(value: new CreatePaymentResponse(Guid.NewGuid()));
});

app.MapPost("psp/api/payments/{paymentId:guid}/settle", () =>
{
    return Results.Ok();
});

app.MapPost("notifications/api/send", (NotificationRequest request) =>
{
    return Results.Ok();
});

app.Run();

internal record CreatePaymentRequest(decimal Amount, string Currency);

internal record CreatePaymentResponse(Guid PaymentId);

internal record NotificationRequest(Guid Id, string JsonPayload);