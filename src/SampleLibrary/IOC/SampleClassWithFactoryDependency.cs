using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    public class SampleClassWithFactoryDependency
    {
        public SampleClassWithFactoryDependency(IFactory<ISampleService> factory)
        {
            Factory = factory;
        }

        public IFactory<ISampleService> Factory { get; }
    }
}