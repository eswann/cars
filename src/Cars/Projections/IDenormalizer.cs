using System;
using System.Threading.Tasks;
using Cars.Handlers;

namespace Cars.Projections
{
    public interface IDenormalizer : IEventHandlerController
    {
        Task RebuildAsync<TProjection>(DateTime? fromTimestamp = null) where TProjection : IProjection;
    }
}