using FluentTesting;

namespace Regis.Pay.EndToEndTests;

public class PaymentTests(RegisPayFixture fixture) : IClassFixture<RegisPayFixture>
{
    private readonly TestSteps _testSteps = new(fixture);

    [Fact]
    public async Task SuccessfullyCompletedPayment()
    {
        await _testSteps
                .Given(c => c.ACreatePaymentRequest())
                .When(c => c.TheCreatePaymentIsRequested())
                .Then(c => c.ThePaymentIsSuccessfullyCompleted())
                .RunAsync();
    }
}