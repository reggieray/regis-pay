namespace Regis.Pay.Api.Endpoints.CreatePayment
{
    public record CreatePaymentRequest(decimal Amount, string Currency);
}
