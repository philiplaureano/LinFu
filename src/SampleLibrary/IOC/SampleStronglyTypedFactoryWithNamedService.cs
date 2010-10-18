using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Factory(typeof (ISampleService), ServiceName = "Test")]
    public class SampleStronglyTypedFactoryWithNamedService : IFactory<ISampleService>
    {
        #region IFactory<ISampleService> Members

        public ISampleService CreateInstance(IFactoryRequest request)
        {
            return new SampleClass();
        }

        #endregion
    }
}