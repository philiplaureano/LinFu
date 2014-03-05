using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Factory(typeof (ISampleService), ServiceName = "Test")]
    public class SampleStronglyTypedFactoryWithNamedService : IFactory<ISampleService>
    {
        public ISampleService CreateInstance(IFactoryRequest request)
        {
            return new SampleClass();
        }
    }
}