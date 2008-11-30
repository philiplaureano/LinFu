namespace SampleLibrary
{
    public interface ISampleGenericService<T>
    {
        bool Called { get; }
    }
}