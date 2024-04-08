using Refit;

namespace Regis.Pay.Tests.Shared.ApiClient
{
    public interface IRegisPayApiClient
    {
        [Post("/api/payment/create")]
        Task<ApiResponse<CreatePaymentResponse>> CreatePayment(CreatePaymentRequest createPayment);
    }
}
