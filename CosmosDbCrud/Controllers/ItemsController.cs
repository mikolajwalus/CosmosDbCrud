using CosmosDbCrud.Data;
using CosmosDbCrud.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemsRepository;

        public ItemsController( IItemRepository itemsRepository)
        {
            _itemsRepository = itemsRepository ?? throw new ArgumentNullException(nameof(itemsRepository));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Item>> GetItems()
        {
            return await _itemsRepository.GetMultipleAsync("SELECT * FROM c");
        }
    }
}
