using CosmosDbCrud.Data.CosmosDbRepository;
using CosmosDbCrud.Data.Entities;

namespace CosmosDbCrud.Data
{
    public interface IItemRepository : ICosmosDbRepository<Item>
    {

    }
}
