using FastEndpoints;
using FluentValidation;

namespace Regis.Pay.Api.Endpoints.CreatePayment
{
    public class CreatePaymentRequestValidator : Validator<CreatePaymentRequest>
    {
        private readonly string[] SupportedCurrencies = ["GBP", "EUR", "USD"];

        public CreatePaymentRequestValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty()
                .WithMessage("Amount must not be empty")
                .GreaterThan(0)
                .WithMessage("Amount must be a postive value");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency must not be empty")
                .Must(x => SupportedCurrencies.Contains(x))
                .WithMessage($"Please use one of these suppored currencies: {string.Join(", ", SupportedCurrencies)}");
        }
    }
}
