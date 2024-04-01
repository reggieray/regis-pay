using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Regis.Pay.Common.Configuration;

namespace Regis.Pay.Common.EventStore
{
    public class CosmosEventStore : IEventStore
    {
        private readonly IEventTypeResolver _eventTypeResolver;
        private readonly CosmosClient _client;
        private readonly CosmosConfigOptions _cosmosConfigOptions;

        public CosmosEventStore(
            IEventTypeResolver eventTypeResolver, 
            CosmosClient client,
            CosmosConfigOptions cosmosConfigOptions)
        {
            _eventTypeResolver = eventTypeResolver;
            _client = client;
            _cosmosConfigOptions = cosmosConfigOptions;
        }

        public async Task<EventStream> LoadStreamAsync(string streamId)
        {
            Container container = _client.GetContainer(_cosmosConfigOptions.DatabaseName, _cosmosConfigOptions.ContainerName);

            var sqlQueryText = "SELECT * FROM events e"
                + " WHERE e.stream.id = @streamId"
                + " ORDER BY e.stream.version";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText)
                .WithParameter("@streamId", streamId);

            int version = 0;
            var events = new List<IDomainEvent>();

            FeedIterator<EventWrapper> feedIterator = container.GetItemQueryIterator<EventWrapper>(queryDefinition);
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<EventWrapper> response = await feedIterator.ReadNextAsync();
                foreach (var eventWrapper in response)
                {
                    version = eventWrapper.StreamInfo.Version;

                    events.Add(eventWrapper.GetEvent(_eventTypeResolver));
                }
            }

            return new EventStream(streamId, version, events);
        }

        public async Task<bool> AppendToStreamAsync(string streamId, int expectedVersion, IEnumerable<IDomainEvent> events)
        {
            Container container = _client.GetContainer(_cosmosConfigOptions.DatabaseName, _cosmosConfigOptions.ContainerName);

            var partitionKey = new PartitionKey(streamId);

            dynamic[] parameters =
            [
                streamId,
                expectedVersion,
                SerializeEvents(streamId, expectedVersion, events)
            ];

            return await container.Scripts.ExecuteStoredProcedureAsync<bool>("spAppendToStream", partitionKey, parameters);
        }

        private static string SerializeEvents(string streamId, int expectedVersion, IEnumerable<IDomainEvent> events)
        {
            var items = events.Select(e => new EventWrapper
            {
                Id = $"{streamId}:{++expectedVersion}",
                StreamInfo = new StreamInfo
                {
                    Id = streamId,
                    Version = expectedVersion
                },
                EventType = e.GetType().Name,
                EventData = JObject.FromObject(e)
            });

            return JsonConvert.SerializeObject(items);
        }
    }
}
