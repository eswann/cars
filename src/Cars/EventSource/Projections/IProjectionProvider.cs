namespace Cars.EventSource.Projections
{
    public interface IProjectionProvider
    {
        object CreateProjection(IAggregateMutator mutator);
    }
}
