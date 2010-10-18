namespace SampleLibrary.IOC
{
    public class SampleClassWithSingleArgumentConstructor
    {
        public SampleClassWithSingleArgumentConstructor(ISampleService service)
        {
            Service = service;
        }

        public ISampleService Service { get; set; }
    }
}