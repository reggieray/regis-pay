using FastEndpoints;
using FluentAssertions;
using ITLIBRIUM.BddToolkit;
using Moq;
using Regis.Pay.Api.Endpoints.CreatePayment;
using Regis.Pay.Common.EventStore;
using System.Net.Http.Json;

namespace Regis.Pay.Api.UnitTests.Endpoints.CreatePayment
{
    public class CreatePaymentScenarioTests
    {
        [Theory]
        [MemberData(nameof(ValidationErrorResponseTestData))]
        public void ValidationErrorResponse((CreatePaymentRequest request, Dictionary<string, List<string>> expectedErrors) scenario)
        {
            Bdd.Scenario<Context>()
                .Given(c => c.ACreatePaymentRequest(scenario.request))
                .When(c => c.TheApiRequestIsMade())
                .Then(c => c.AValidationErrorResponseIsReturned(scenario.expectedErrors))
                .Test();
        }

        [Theory]
        [InlineData("GBP")]
        [InlineData("EUR")]
        [InlineData("USD")]
        public void SuccessfulCreatePayment(string currency)
        {
            Bdd.Scenario<Context>()
                .Given(c => c.ACreatePaymentRequest(new CreatePaymentRequest(100, currency)))
                .When(c => c.TheApiRequestIsMade())
                .Then(c => c.APaymentIdIsReturned())
                .Test();
        }

        public static IEnumerable<object[]> ValidationErrorResponseTestData =>
        new List<object[]>
        {
            new object[] { CreatePaymentScenarioBuilder.EmptyAmount() },
            new object[] { CreatePaymentScenarioBuilder.InvaildAmount() },
            new object[] { CreatePaymentScenarioBuilder.EmptyCurrency() },
            new object[] { CreatePaymentScenarioBuilder.InvaildCurrency() }
        };

        private class Context
        {
            private const string CreateEndpoint = "api/payment/create";
            private readonly HttpClient _createApiClient;
            private CreatePaymentRequest _paymentRequest = default!;
            private HttpResponseMessage _response = default!;
            private RegisPayApi _regisPayApi = default!;

            public Context()
            {
                _regisPayApi = new RegisPayApi();
                _createApiClient = _regisPayApi.CreateClient();

                _regisPayApi.MockEventStore.Setup(x => x.AppendToStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IDomainEvent>>()))
                    .ReturnsAsync(true);
            }

            internal void ACreatePaymentRequest(CreatePaymentRequest request)
            {
                _paymentRequest = request;
            }

            internal async Task APaymentIdIsReturned()
            {
                _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var response = await _response.Content.ReadFromJsonAsync<CreatePaymentResponse>();
                response!.PaymentId.Should().NotBeEmpty();
            }

            internal async Task AValidationErrorResponseIsReturned(Dictionary<string, List<string>> expectedErrors)
            {
                _response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
                var errorResponse = await _response.Content.ReadFromJsonAsync<ErrorResponse>();
                errorResponse!.Errors.Should().BeEquivalentTo(expectedErrors);
            }

            internal async Task TheApiRequestIsMade()
            {
                _response = await _createApiClient
                    .PostAsJsonAsync(CreateEndpoint, _paymentRequest)
                    .ConfigureAwait(false);
            }
        }
    }
}
