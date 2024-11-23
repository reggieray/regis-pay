using FluentAssertions;
using FluentTesting;
using Refit;
using Regis.Pay.Domain.IntegrationEvents;
using Regis.Pay.Tests.Shared.ApiClient;
using Regis.Pay.Tests.Shared.EventTestConsumer.EventTestConsumer;
namespace Regis.Pay.EndToEndTests;

public class PaymentTests
{
    private readonly TestSteps _testSteps = new();
    
    [Fact]
    public async Task SuccessfullyCompletedPayment()
    {
        await _testSteps
                .Given(c => c.ACreatePaymentRequest())
                .When(c => c.TheCreatePaymentIsRequested())
                .Then(c => c.ThePaymentIsSuccessfullyCompleted())
                .RunAsync();
    }

    private class TestSteps
    {
        private readonly IRegisPayApiClient _regisPayApiClient;
        private CreatePaymentRequest _createPaymentRequest;
        private readonly PaymentCompletedEventTestConsumer _testConsumer;
        private PaymentCompleted _paymentCompleted;
        private CreatePaymentResponse _createPaymentResponse;

        public TestSteps()
        {
            _regisPayApiClient = RestService.For<IRegisPayApiClient>("https://localhost:4433");
            _testConsumer = new PaymentCompletedEventTestConsumer();
        }

        internal void ACreatePaymentRequest()
        {
            _createPaymentRequest = new CreatePaymentRequest(130, "GBP");
        }

        internal async Task TheCreatePaymentIsRequested()
        {
            _paymentCompleted = await _testConsumer.ListenToEvent(async () =>
            {
                var response = await _regisPayApiClient.CreatePayment(_createPaymentRequest);
                await response.EnsureSuccessStatusCodeAsync();
                _createPaymentResponse = response.Content!;
            });
        }

        internal void ThePaymentIsSuccessfullyCompleted()
        {
            _paymentCompleted.Should().NotBeNull();
            _paymentCompleted.AggregateId.Should().Be($"pay:{_createPaymentResponse.PaymentId}");
        }
    }
}