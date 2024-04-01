using Newtonsoft.Json;

namespace Regis.Pay.Common.EventStore
{
    public class StreamInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
