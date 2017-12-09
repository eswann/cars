using System;

namespace Cars.EventStore.MongoDB
{
    public static class MongoEventStoreSettingsValidator
    {
        public static void Validate(this IMongoEventStoreSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Database))
                throw new ArgumentNullException(nameof(settings.Database));

            if (string.IsNullOrWhiteSpace(settings.EventsCollectionName))
                throw new ArgumentNullException(nameof(settings.EventsCollectionName));

            if (string.IsNullOrWhiteSpace(settings.ProjectionsCollectionName))
                throw new ArgumentNullException(nameof(settings.ProjectionsCollectionName));
        }
    }
}
