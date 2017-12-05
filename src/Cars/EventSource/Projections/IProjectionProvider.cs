namespace Cars.EventSource.Projections
{
    public interface IProjectionProvider
    {
        object CreateProjection(IMutator mutator);
    }
}
