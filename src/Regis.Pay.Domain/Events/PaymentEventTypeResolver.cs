using Regis.Pay.Common.EventStore;

namespace Regis.Pay.Domain.Events
{
    public class PaymentEventTypeResolver : IEventTypeResolver
    {
        public Type GetEventType(string typeName)
        {
            return Type.GetType($"Regis.Pay.Domain.Events.{typeName}, Regis.Pay.Domain");
        }
    }
}
