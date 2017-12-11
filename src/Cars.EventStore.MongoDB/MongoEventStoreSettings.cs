namespace Cars.EventStore.MongoDB
{
    public class MongoEventStoreSettings : IMongoEventStoreSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string Database { get; set; } = "EventStore";
        public string EventsCollectionName { get; set; } = "Events";
        public string ProjectionsCollectionName { get; set; } = "Projections";
    }

}