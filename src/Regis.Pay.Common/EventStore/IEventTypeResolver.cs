namespace Regis.Pay.Common.EventStore
{
    public interface IEventTypeResolver
    {
        Type GetEventType(string typeName);
    }
}
