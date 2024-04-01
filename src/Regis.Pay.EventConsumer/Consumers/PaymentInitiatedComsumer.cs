using MassTransit;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentInitiatedComsumer : IConsumer<PaymentInitiated>
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentInitiatedComsumer(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task Consume(ConsumeContext<PaymentInitiated> context)
        {
            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            await Task.Delay(300); // Do some process here on payment initiated. eg. save to SQL database or third party system.

            payment.Created();

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
