namespace CosmosDbCrud.Data.CosmosDbRepository
{
    public interface ICosmosDbRepository<T> where T : class, ICosmosDbItem
    {
        Task AddAsync(T item);
        Task DeleteAsync(string id, string partitionKey);
        Task<T> GetAsync(string id, string partitionKey);
        Task<IEnumerable<T>> GetMultipleAsync(string queryString);
        Task UpdateAsync(T item);
    }
}