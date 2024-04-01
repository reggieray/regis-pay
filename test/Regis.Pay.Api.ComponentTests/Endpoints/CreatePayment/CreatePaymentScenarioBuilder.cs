using Regis.Pay.Api.Endpoints.CreatePayment;

namespace Regis.Pay.Api.UnitTests.Endpoints.CreatePayment
{
    public static class CreatePaymentScenarioBuilder
    {
        public static (CreatePaymentRequest request, Dictionary<string, List<string>> expectedErrors) EmptyAmount()
        {
            var request = new CreatePaymentRequest(default, "EUR");
            var expectedErrors = new Dictionary<string, List<string>>
            {
                { "amount", new List<string> { "Amount must not be empty", "Amount must be a postive value" } }
            };

            return (request, expectedErrors);
        }

        public static (CreatePaymentRequest request, Dictionary<string, List<string>> expectedErrors) InvaildAmount()
        {
            var request = new CreatePaymentRequest(-1, "EUR");
            var expectedErrors = new Dictionary<string, List<string>>
            {
                { "amount", new List<string> { "Amount must be a postive value" } }
            };

            return (request, expectedErrors);
        }

        public static (CreatePaymentRequest request, Dictionary<string, List<string>> expectedErrors) EmptyCurrency()
        {
            var request = new CreatePaymentRequest(100, default!);
            var expectedErrors = new Dictionary<string, List<string>>
            {
                { "currency", new List<string> { "Currency must not be empty", "Please use one of these suppored currencies: GBP, EUR, USD" } }
            };

            return (request, expectedErrors);
        }

        public static (CreatePaymentRequest request, Dictionary<string, List<string>> expectedErrors) InvaildCurrency()
        {
            var request = new CreatePaymentRequest(100, "YEN");
            var expectedErrors = new Dictionary<string, List<string>>
            {
                { "currency", new List<string> { "Please use one of these suppored currencies: GBP, EUR, USD" } }
            };

            return (request, expectedErrors);
        }
    }
}
