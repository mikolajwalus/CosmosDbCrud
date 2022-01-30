namespace CosmosDbCrud.Data
{
    public interface ICosmosDbItem
    {
        /// <summary>
        /// Unique identifier for all objects
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Partition Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Document Type
        /// </summary>
        public string Type { get; set; }
    }
}
