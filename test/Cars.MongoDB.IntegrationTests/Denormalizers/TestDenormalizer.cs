using System.Threading.Tasks;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Storage;
using Cars.EventStore.MongoDB.Projections;
using Cars.Handlers;
using Cars.Projections;

namespace Cars.MongoDB.IntegrationTests.Denormalizers
{
    public class TestDenormalizer : MongoDenormalizer, IEventHandler<ThingCreated>, IEventHandler<ThingHappened>
    {
        private readonly IProjectionRepository _repository;
        public TestDenormalizer(IProjectionRepository repository, IEventStore eventStore, IEventSerializer serializer) : base(repository, eventStore, serializer)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(ThingCreated @event)
        {
            var projection = new TestProjection();

            await _repository.UpsertAsync(projection, @event);
        }

        public async Task ExecuteAsync(ThingHappened @event)
        {
            var projection = await _repository.RetrieveAsync<TestProjection>("TestDenormalizedProjection");
            projection.LastThing = @event.Text;
            await _repository.UpsertAsync(projection, @event);
        }

        public async Task RebuildAsync()
        {
            await RebuildAsync<TestProjection>();
        }
    }
}
