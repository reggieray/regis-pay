using Refit;

namespace Regis.Pay.Common.ApiClients.Payments
{
    public interface IPaymentsApi
    {
        [Post("/psp/api/payments/create")]
        Task<ApiResponse<CreatePaymentResponse>> CreatePaymentAsync([Body] CreatePaymentRequest request);

        [Post("/psp/api/payments/{paymentId}/settle")]
        Task<HttpResponseMessage> SettlePaymentAsync(Guid paymentId);
    }
}
