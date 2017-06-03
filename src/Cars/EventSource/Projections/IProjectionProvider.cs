namespace Cars.EventSource.Projections
{
    public interface IProjectionProvider
    {
        object CreateProjection(IStream stream);
    }
}
