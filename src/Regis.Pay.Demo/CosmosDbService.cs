using Microsoft.Azure.Cosmos;

namespace Regis.Pay.Demo
{
    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosDbService(string account, string key, string databaseId, string containerId)
        {
            _cosmosClient = new CosmosClient(account, key);
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<List<T>> GetItemsAsync<T>(string queryString)
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            var results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
