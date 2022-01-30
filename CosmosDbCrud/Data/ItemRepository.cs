using CosmosDbCrud.Data.CosmosDbRepository;
using CosmosDbCrud.Data.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosDbCrud.Data
{
    public class ItemRepository : CosmosDbRepository<Item>, IItemRepository
    {
        public ItemRepository(CosmosClient cosmosDbClient, string databaseName, string containerName) : base(cosmosDbClient, databaseName, containerName)
        {
        }
    }
}
