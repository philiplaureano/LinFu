namespace SampleLibrary.IOC
{
    public class SampleClassWithServiceArrayAsConstructorArgument
    {
        private readonly ISampleService[] _services;

        public SampleClassWithServiceArrayAsConstructorArgument(ISampleService[] services)
        {
            _services = services;
        }

        public ISampleService[] Services
        {
            get { return _services; }
        }
    }
}