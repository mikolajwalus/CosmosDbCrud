using CosmosDbCrud.Data;

namespace CosmosDbCrud.Controllers
{
    public class ItemsController
    {
        private readonly IItemRepository _itemsRepository;

        public ItemsController( IItemRepository itemsRepository)
        {
            _itemsRepository = itemsRepository ?? throw new ArgumentNullException(nameof(itemsRepository));
        }


    }
}
