namespace Cars.EventStore.MongoDB
{
    public interface IMongoEventStoreSettings
    {
        string ConnectionString { get; }
        string Database { get; }
        string EventsCollectionName { get; }
        string ProjectionsCollectionName { get; }
    }
}