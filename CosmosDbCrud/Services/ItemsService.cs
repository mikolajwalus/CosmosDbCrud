using CosmosDbCrud.Data.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosDbCrud.Services
{
    public class ItemsService : IItemsService
    {
        private Container _container;

        public ItemsService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(Item item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Type));
        }

        public async Task DeleteAsync(string id, string partitionKey)
        {
            await _container.DeleteItemAsync<Item>(id, new PartitionKey(partitionKey));
        }

        public async Task<Item> GetAsync(string id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<Item>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<Item>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));

            var results = new List<Item>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAsync(Item item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(item.Type));
        }
    }
}
