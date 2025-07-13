namespace Regis.Pay.EndToEndTests;
using System.Net.Http.Json;
using FluentAssertions;
using Domain.IntegrationEvents;
using Tests.Shared.ApiClient;
using Tests.Shared.EventTestConsumer.EventTestConsumer;

public class TestSteps
{
    private readonly RegisPayFixture _fixture;
    private CreatePaymentRequest _createPaymentRequest;
    private readonly PaymentCompletedEventTestConsumer _testConsumer;
    private PaymentCompleted _paymentCompleted;
    private CreatePaymentResponse? _createPaymentResponse;

    public TestSteps(RegisPayFixture fixture)
    {
        _fixture = fixture;
        _testConsumer = new PaymentCompletedEventTestConsumer();
    }

    internal void ACreatePaymentRequest()
    {
        _createPaymentRequest = new CreatePaymentRequest(130, "GBP");
    }

    internal async Task TheCreatePaymentIsRequested()
    {
        // Setting up a queue listener to verify, other options to verify are available such as checking the completed event is in the DB
        // or creating a test harness for the notification that gets send at the end 
        _paymentCompleted = await _testConsumer.ListenToEvent(async () =>
        {
            var response = await _fixture.ApiClient.PostAsJsonAsync("api/payment/create", _createPaymentRequest);
            response.EnsureSuccessStatusCode();
            _createPaymentResponse = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        }, _fixture.RabbitMqConnString!);
    }

    internal void ThePaymentIsSuccessfullyCompleted()
    {
        _paymentCompleted.Should().NotBeNull(because: $"pay:{_createPaymentResponse?.PaymentId} payment was created");
        _paymentCompleted.AggregateId.Should().Be($"pay:{_createPaymentResponse?.PaymentId}");
    }
}