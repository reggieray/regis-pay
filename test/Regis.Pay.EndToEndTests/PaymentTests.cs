using FluentAssertions;
using ITLIBRIUM.BddToolkit;
using Refit;
using Regis.Pay.EndToEndTests.ApiClient;
using Regis.Pay.EndToEndTests.EventTestConsumer;

namespace Regis.Pay.EndToEndTests;

public class PaymentTests
{
    [Fact]
    public void SuccessfullyCompletedPayment()
    {
        Bdd.Scenario<Context>()
                .Given(c => c.ACreatePaymentRequest())
                .When(c => c.TheCreatePaymentIsRequested())
                .Then(c => c.ThePaymentIsSuccessfullyCompleted())
                .Test();
    }

    private class Context
    {
        private readonly IRegisPayApiClient _regisPayApiClient;
        private CreatePaymentRequest _createPaymentRequest;
        private PaymentCompletedEventTestConsumer _testConsumer;
        private PaymentCompleted _paymentCompleted;
        private CreatePaymentResponse _createPaymentResponse;

        public Context()
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