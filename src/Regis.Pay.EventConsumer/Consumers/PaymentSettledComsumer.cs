using MassTransit;
using Regis.Pay.Domain;
using Regis.Pay.Domain.IntegrationEvents;

namespace Regis.Pay.EventConsumer.Consumers
{
    public class PaymentSettledComsumer : IConsumer<PaymentSettled>
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentSettledComsumer(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task Consume(ConsumeContext<PaymentSettled> context)
        {
            var payment = await _paymentRepository.LoadAsync(context.Message.AggregateId);

            await Task.Delay(300); // Do some process here on payment settled. eg. send out webhook.

            payment.Complete();

            await _paymentRepository.SaveAsync(payment);
        }
    }
}
