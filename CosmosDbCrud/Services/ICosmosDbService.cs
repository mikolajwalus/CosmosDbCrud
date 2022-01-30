using CosmosDbCrud.Data.Entities;

namespace CosmosDbCrud.Services
{
    public interface ICosmosDbService
    {
        Task AddAsync(Item item);
        Task DeleteAsync(string id, string partitionKey);
        Task<Item> GetAsync(string id, string partitionKey);
        Task<IEnumerable<Item>> GetMultipleAsync(string queryString);
        Task UpdateAsync(Item item);
    }
}