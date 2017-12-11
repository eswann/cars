using System;

namespace Cars.Projections
{
    public class ProjectionMetadata : IProjectionMetadata
    {
        public DateTime Timestamp { get; set; } = DateTime.MinValue;
    }
}
