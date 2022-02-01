using CosmosDbCrud.Data;
using CosmosDbCrud.Data.Seed;
using CosmosDbCrud.Services;
using Microsoft.Azure.Cosmos;

namespace CosmosDbCrud.Infrastructure
{
    public static class CosmosDb
    {
        public static async Task AddCosmosDbClient(this IServiceCollection services, IConfiguration config)
        {
            var cosmosConfig = GetCosmosDbConfig(config);

            var account = cosmosConfig["Account"];
            var key = cosmosConfig["Key"];

            var client = new CosmosClient(account, key);

            await InitializeDatabasesAndContainers(client, config);

            services.AddSingleton<CosmosClient>(client);
        }

        public static void AddRepositories(this IServiceCollection services, IConfiguration config)
        {
            var cosmosConfig = GetCosmosDbConfig(config);

            services.AddItemsRepository(cosmosConfig);
        }

        /// <summary>
        /// Ensures that all databases and container are created
        /// </summary>
        private static async Task InitializeDatabasesAndContainers(CosmosClient client, IConfiguration config)
        {
            var cosmosConfig = GetCosmosDbConfig(config);

            var itemsDatabase = cosmosConfig["ItemsDatabase"];
            var itemsContainer = cosmosConfig["ItemsContainer"];

            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(itemsDatabase);
            await database.Database.CreateContainerIfNotExistsAsync(itemsContainer, "/type");
        }

        private static void AddItemsRepository(this IServiceCollection services, IConfigurationSection config)
        {
            var itemsDatabase = config["ItemsDatabase"];
            var itemsContainer = config["ItemsContainer"];

            services.AddScoped<IItemRepository, ItemRepository>(services => 
                new ItemRepository(services.GetService<CosmosClient>(), itemsDatabase, itemsContainer, services.GetService<ILogger<ItemRepository>>()));
        }

        private static IConfigurationSection GetCosmosDbConfig(IConfiguration config) => config.GetSection("CosmosDb");
    }
}
