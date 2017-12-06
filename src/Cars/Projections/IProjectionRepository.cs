using System;
using System.Threading.Tasks;

namespace Cars.Projections
{
    public interface IProjectionRepository
    {
        Task Insert<TProjection>(TProjection projection) where TProjection : IProjection;

        Task Update<TProjection>(TProjection projection) where TProjection : IProjection;

        Task<TProjection> Retrieve<TProjection>(string projectionId) where TProjection : IProjection;
    }
}