namespace SampleLibrary.IOC
{
    public interface ISampleService<T>
    {
        int Int { get; }
        string Text { get; }
        bool Bool { get; }
    }
}