using Microsoft.Extensions.DependencyInjection;
using Regis.Pay.Common.EventStore;
using Regis.Pay.Domain.Events;

namespace Regis.Pay.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDomain(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentRepository, PaymentRepository>();
            services.AddSingleton<IEventTypeResolver, PaymentEventTypeResolver>();
        }
    }
}
