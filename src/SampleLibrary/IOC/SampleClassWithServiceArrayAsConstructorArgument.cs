namespace SampleLibrary.IOC
{
    public class SampleClassWithServiceArrayAsConstructorArgument
    {
        public SampleClassWithServiceArrayAsConstructorArgument(ISampleService[] services)
        {
            Services = services;
        }

        public ISampleService[] Services { get; }
    }
}