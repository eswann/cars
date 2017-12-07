using System.Threading.Tasks;

namespace Cars.Projections
{
    public interface IProjectionRepository
    {
        Task InsertAsync<TProjection>(TProjection projection) where TProjection : IProjection;

        Task UpdateAsync<TProjection>(TProjection projection) where TProjection : IProjection;

        Task<TProjection> RetrieveAsync<TProjection>(object projectionId) where TProjection : IProjection;
    }
}