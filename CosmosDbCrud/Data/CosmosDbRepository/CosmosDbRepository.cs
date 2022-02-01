using Microsoft.Azure.Cosmos;

namespace CosmosDbCrud.Data.CosmosDbRepository
{
    public abstract class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : class, ICosmosDbItem
    {
        private readonly ILogger _logger;
        private Container _container;

        public CosmosDbRepository(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName, 
            ILogger logger)
        {
            if (cosmosDbClient is null)
            {
                throw new ArgumentNullException(nameof(cosmosDbClient));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException($"'{nameof(databaseName)}' cannot be null or empty.", nameof(databaseName));
            }

            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentException($"'{nameof(containerName)}' cannot be null or empty.", nameof(containerName));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(T item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Key));
        }

        public async Task DeleteAsync(string id, string partitionKey)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public async Task<T> GetAsync(string id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException) //TODO: handle not found
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));

            var results = new List<T>();

            LogOperationInfo(nameof(GetMultipleAsync));

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
                LogRequestUnits(response);
            }

            return results;
        }

        public async Task UpdateAsync(T item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(item.Key));
        }

        private void LogOperationInfo(string operation)
        {
            _logger.LogInformation($"Logging chagre for: \nOperation: {operation}");
        }

        private void LogRequestUnits(FeedResponse<T> response)
        {
            _logger.LogInformation($"Count: {response.Count}\nRequestCharge: {response.RequestCharge}");
        }
    }
}
