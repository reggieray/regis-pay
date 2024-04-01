namespace Regis.Pay.Domain
{
    public interface IPaymentRepository
    {
        Task<Payment> LoadAsync(string streamId);

        Task<bool> SaveAsync(Payment payment);
    }
}
