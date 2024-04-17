using FluentAssertions;
using ITLIBRIUM.BddToolkit;
using MassTransit.Testing;
using Moq;
using Regis.Pay.Common.EventStore;
using Regis.Pay.Domain.IntegrationEvents;
using Regis.Pay.EventConsumer.Consumers;

namespace Regis.Pay.EventConsumer.ComponentTests.Consumers
{
    public class PaymentInitiatedComsumerTests
    {
        [Fact]
        public void ShouldCreatePaymentOnPaymentInitiated()
        {
            Bdd.Scenario<Context>()
                .Given(c => c.APaymentInitiatedIntegrationEvent())
                .When(c => c.ThePaymentInitiatedIntegrationEventIsConsumed())
                .Then(c =>c.ThePaymentCreatedDomainEventIsPersistedToPayment())
                .Test();
        }

        private class Context
        {
            private readonly EventConsumerWorker _eventConsumerWorker = default!;
            private PaymentInitiated _paymentInitiated = default!;
            private readonly ITestHarness _massTransitTestHarness = default!;
            private readonly Guid paymentId = Guid.NewGuid();

            public Context()
            {
                _eventConsumerWorker = new EventConsumerWorker();

                _eventConsumerWorker.MockEventStore.Setup(x => x.LoadStreamAsync(It.Is<string>(x => x == $"pay:{paymentId}")))
                    .ReturnsAsync(new EventStream($"pay:{paymentId}", 1, new List<IDomainEvent>() { new Domain.Events.PaymentInitiated { PaymentId = paymentId, Amount = 10, Currency = "EUR", Timestamp = DateTime.UtcNow.AddMinutes(-1) } }));

                _eventConsumerWorker.MockEventStore.Setup(x => x.AppendToStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IDomainEvent>>()))
                    .ReturnsAsync(true);

                _massTransitTestHarness = _eventConsumerWorker.Services.GetTestHarness();
            }

            internal void APaymentInitiatedIntegrationEvent()
            {
                _paymentInitiated = new PaymentInitiated() { AggregateId = $"pay:{paymentId}" };
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
                _eventConsumerWorker.MockEventStore.Verify(x => x.AppendToStreamAsync(It.Is<string>(streamId => streamId == $"pay:{paymentId}"), It.IsAny<int>(), It.Is<IEnumerable<IDomainEvent>>(events => VerifyEvents(events))));
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
}
