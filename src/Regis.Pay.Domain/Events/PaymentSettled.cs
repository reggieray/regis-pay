namespace Regis.Pay.Domain.Events
{
    public class PaymentSettled : DomainEventBase
    {
        public Guid PaymentReference { get; set; }
    }
}
