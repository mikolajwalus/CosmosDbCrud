using CosmosDbCrud.Data.CosmosDbRepository;
using Newtonsoft.Json;

namespace CosmosDbCrud.Data.Entities
{
    public class Item : ICosmosDbItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("isComplete")]
        public bool Completed { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
