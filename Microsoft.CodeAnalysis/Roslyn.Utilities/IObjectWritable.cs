namespace Roslyn.Utilities
{
    public interface IObjectWritable
    {
        bool ShouldReuseInSerialization { get; }

        void WriteTo(ObjectWriter writer);
    }
}
