namespace Cars.Projections
{
    public abstract class ProjectionBase : IProjection
    {
        public virtual string ProjectionId { get; protected set; }

        public IProjectionMetadata Metadata { get; protected set; } = new ProjectionMetadata();
    }
}
