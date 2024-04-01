using FastEndpoints;
using Regis.Pay.Api.Endpoints.CreatePayment;
using Regis.Pay.Domain;

namespace Regis.Pay.Api.Endpoints.Create
{
    public class CreatePaymentEndpoint : Endpoint<CreatePaymentRequest, CreatePaymentResponse>
    {
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentEndpoint(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public override void Configure()
        {
            Post("api/payment/create");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreatePaymentRequest req, CancellationToken ct)
        {
            var payment = new Payment(Guid.NewGuid(), req.Amount, req.Currency);

            await _paymentRepository.SaveAsync(payment);

            await SendAsync(new CreatePaymentResponse(payment.PaymentId), cancellation: ct);
        }
    }
}
