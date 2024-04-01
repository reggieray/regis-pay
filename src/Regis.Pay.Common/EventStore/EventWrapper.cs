using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Regis.Pay.Common.EventStore
{
    public class EventWrapper
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("stream")]
        public StreamInfo StreamInfo { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventData")]
        public JObject EventData { get; set; }

        public IDomainEvent GetEvent(IEventTypeResolver eventTypeResolver)
        {
            Type eventType = eventTypeResolver.GetEventType(EventType);

            return (IDomainEvent)EventData.ToObject(eventType);
        }
    }
}
