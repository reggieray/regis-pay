using System.Text.Json;
using System.Text;

namespace Regis.Pay.Demo
{
    public class RegisPayApiClient(HttpClient httpClient)
    {
        public async Task<RegisPayPaymentResponse> CreatePayment(RegisPayPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/payment/create", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RegisPayPaymentResponse>(cancellationToken) ?? throw new Exception("Create Payment Api response is null");
        }
    }

    public class RegisPayPaymentRequest
    {
        public decimal Amount { get; set; } = 108.1M;
        public string Currency { get; set; } = "EUR";
    };

    public record RegisPayPaymentResponse(Guid PaymentId);
}
