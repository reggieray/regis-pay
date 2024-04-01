namespace Regis.Pay.Domain.Events
{
    public class PaymentInitiated : DomainEventBase
    {
        public Guid PaymentId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}
