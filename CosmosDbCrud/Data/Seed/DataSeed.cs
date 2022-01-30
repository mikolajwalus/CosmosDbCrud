using Bogus;
using CosmosDbCrud.Data.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosDbCrud.Data.Seed
{
    public static class DataSeed
    {
        public async static Task Seed(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var client = serviceProvider.GetService<CosmosClient>();
            var logger = serviceProvider.GetService<ILogger<Program>>();

            var cosmosSettings = configuration.GetSection("CosmosDb");
            var databaseName = cosmosSettings["ItemsDatabase"];
            var containerName = cosmosSettings["ItemsContainer"];

            var container = client.GetContainer(databaseName, containerName);

            var hasData = await CheckIfHasData(container);

            if (!hasData)
            {
                logger.LogInformation("No data in database, seeding");

                var itemsToInsert = GenerateItems();

                await InsertItemsToDatabase(container, logger, itemsToInsert);
            }

            logger.LogInformation("Data present in database no need for seed");
        }

        private static async Task<bool> CheckIfHasData(Container container)
        {
            var query = container.GetItemQueryIterator<dynamic>(new QueryDefinition("SELECT * FROM c"));

            var response = await query.ReadNextAsync();

            if (response.Count() > 0) return true;

            return false;
        }

        private static List<Item> GenerateItems() => 
            new Faker<Item>()
                .RuleFor(f => f.Id, f => Guid.NewGuid().ToString())
                .RuleFor(f => f.Name, f => f.Person.FirstName)
                .RuleFor(f => f.Description, f => f.Lorem.Sentences(2))
                .RuleFor(f => f.Completed, f => f.Random.Bool())
                .RuleFor(f => f.Key, f => "Item")
                .RuleFor(f => f.Type, f => "Item")
                .Generate(1000);

        private static async Task InsertItemsToDatabase(Container container, ILogger logger, List<Item> itemsToInsert)
        {
            List<Task> tasks = new List<Task>(itemsToInsert.Count);

            foreach (Item item in itemsToInsert)
            {
                tasks.Add(container.CreateItemAsync(item, new PartitionKey(item.Type))
                    .ContinueWith(itemResponse =>
                    {
                        if (!itemResponse.IsCompletedSuccessfully)
                        {
                            AggregateException innerExceptions = itemResponse.Exception.Flatten();
                            if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                            {
                                logger.LogError($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                            }
                            else
                            {
                                logger.LogError($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                            }
                        }
                    }));
            }

            // Wait until all are done
            await Task.WhenAll(tasks);
        }
    }
}
