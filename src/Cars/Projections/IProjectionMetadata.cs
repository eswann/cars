using System;

namespace Cars.Projections
{
    public interface IProjectionMetadata
    {
        DateTime Timestamp { get; }
    }
}