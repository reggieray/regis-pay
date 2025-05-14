namespace Regis.Pay.Common.ApiClients.Payments
{
    public record CreatePaymentRequest(decimal Amount, string Currency);
}
