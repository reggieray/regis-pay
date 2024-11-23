using FluentAssertions;
using FluentTesting;
using MassTransit.Testing;
using Moq;
using Regis.Pay.Common.EventStore;
using Regis.Pay.Domain.IntegrationEvents;
using Regis.Pay.EventConsumer.Consumers;

namespace Regis.Pay.EventConsumer.ComponentTests.Consumers;

public class PaymentInitiatedConsumerTests
{
    private readonly TestSteps _testSteps = new();
        
    [Fact]
    public async Task ShouldCreatePaymentOnPaymentInitiated()
    {
        await _testSteps
            .Given(c => c.APaymentInitiatedIntegrationEvent())
            .When(c => c.ThePaymentInitiatedIntegrationEventIsConsumed())
            .Then(c =>c.ThePaymentCreatedDomainEventIsPersistedToPayment())
            .RunAsync();
    }

    private class TestSteps
    {
        private readonly EventConsumerWorker _eventConsumerWorker;
        private PaymentInitiated _paymentInitiated = default!;
        private readonly ITestHarness _massTransitTestHarness;
        private readonly Guid _paymentId = Guid.NewGuid();

        public TestSteps()
        {
            _eventConsumerWorker = new EventConsumerWorker();

            _eventConsumerWorker.MockEventStore.Setup(x => x.LoadStreamAsync(It.Is<string>(x => x == $"pay:{_paymentId}")))
                .ReturnsAsync(new EventStream($"pay:{_paymentId}", 1, new List<IDomainEvent>() { new Domain.Events.PaymentInitiated { PaymentId = _paymentId, Amount = 10, Currency = "EUR", Timestamp = DateTime.UtcNow.AddMinutes(-1) } }));

            _eventConsumerWorker.MockEventStore.Setup(x => x.AppendToStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IDomainEvent>>()))
                .ReturnsAsync(true);

            _massTransitTestHarness = _eventConsumerWorker.Services.GetTestHarness();
        }

        internal void APaymentInitiatedIntegrationEvent()
        {
            _paymentInitiated = new PaymentInitiated() { AggregateId = $"pay:{_paymentId}" };
        }

        internal async Task ThePaymentInitiatedIntegrationEventIsConsumed()
        {   
            await _massTransitTestHarness.Bus.Publish(_paymentInitiated);

            var consumerTestHarness = _massTransitTestHarness.GetConsumerHarness<PaymentInitiatedConsumer>();

            var consumed = await consumerTestHarness.Consumed.Any<PaymentInitiated>(x => x.Context.Message.AggregateId == _paymentInitiated.AggregateId);

            consumed.Should().BeTrue();
        }

        internal void ThePaymentCreatedDomainEventIsPersistedToPayment()
        {
            _eventConsumerWorker.MockEventStore.Verify(x => x.AppendToStreamAsync(It.Is<string>(streamId => streamId == $"pay:{_paymentId}"), It.IsAny<int>(), It.Is<IEnumerable<IDomainEvent>>(events => VerifyEvents(events))));
        }

        private static bool VerifyEvents(IEnumerable<IDomainEvent> events)
        {
            events.Should().NotBeEmpty();

            var typedEvent = events.First() as Domain.Events.PaymentCreated;

            typedEvent.Should().NotBeNull();

            return true;
        }
    }
}