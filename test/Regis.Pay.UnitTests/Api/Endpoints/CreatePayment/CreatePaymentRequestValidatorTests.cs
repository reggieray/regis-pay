using FluentAssertions;
using Regis.Pay.Api.Endpoints.CreatePayment;

namespace Regis.Pay.Api.UnitTests.Endpoints.CreatePayment
{
    public class CreatePaymentRequestValidatorTests
    {
        [Fact]
        public void ValidationFailures()
        {
            var validator = new CreatePaymentRequestValidator();
            var request = new CreatePaymentRequest(-1, "EUR");

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
        }
    }
}
