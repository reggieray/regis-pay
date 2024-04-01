using Regis.Pay.Common.EventStore;

namespace Regis.Pay.Domain
{
    public class PaymentRepository : IPaymentRepository
    {
        private const string StreamIdPrefix = "pay";
        private readonly IEventStore _eventStore;

        public PaymentRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Payment> LoadAsync(string streamId)
        {
            var stream = await _eventStore.LoadStreamAsync(streamId);

            if (stream is null)
            {
                throw new Exception($"Unable to find payment for streamId: {streamId}"); //Add custom exception
            }

            return new Payment(stream!.Events);
        }

        public async Task<bool> SaveAsync(Payment payment)
        {
            if (payment.Changes.Any())
            {
                var streamId = $"{StreamIdPrefix}:{payment.PaymentId}";

                return await _eventStore.AppendToStreamAsync(
                streamId,
                payment.Version,
                payment.Changes);
            }

            return true;
        }
    }
}
