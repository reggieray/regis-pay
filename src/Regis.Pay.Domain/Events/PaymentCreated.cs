namespace Regis.Pay.Domain.Events
{
    public class PaymentCreated : DomainEventBase
    {
        public Guid PaymentReference { get; set; }
    }
}
