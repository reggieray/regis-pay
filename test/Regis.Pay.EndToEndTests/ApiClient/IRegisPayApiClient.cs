using Refit;

namespace Regis.Pay.EndToEndTests.ApiClient
{
    public interface IRegisPayApiClient
    {
        [Post("/api/payment/create")]
        Task<ApiResponse<CreatePaymentResponse>> CreatePayment(CreatePaymentRequest createPayment);
    }
}
