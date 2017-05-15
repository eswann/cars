namespace Cars.EventStore.MongoDB
{
    public interface IMongoEventStoreSettings
    {
        string Database { get; }
        string EventsCollectionName { get; }
        string ProjectionsCollectionName { get; }
        string SnapshotsCollectionName { get; }
    }
}