using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    public class SampleClassWithArrayPropertyDependency
    {
        [Inject]
        public ISampleService[] Property { get; set; }
    }
}