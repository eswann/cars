using System.Threading.Tasks;
using Cars.Events;

namespace Cars.Projections
{
    public interface IProjectionRepository
    {
        Task UpsertAsync<TProjection>(TProjection projection, IDomainEvent lastEvent) where TProjection : IProjection;

        Task<TProjection> RetrieveAsync<TProjection>(object projectionId) where TProjection : IProjection;
    }
}