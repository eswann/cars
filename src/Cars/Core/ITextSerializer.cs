namespace Cars.Core
{
    public interface ITextSerializer
    {
        string Serialize(object @object);
        object Deserialize(string textSerialized, string type);
        T Deserialize<T>(string textSerialized);
    }
}