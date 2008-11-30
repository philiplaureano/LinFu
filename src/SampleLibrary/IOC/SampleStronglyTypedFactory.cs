using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof (ISampleService))]
    public class SampleStronglyTypedFactory : IFactory<ISampleService>
    {
        #region IFactory<ISampleService> Members

        public ISampleService CreateInstance(IFactoryRequest request)
        {
            return new SampleClass();
        }

        #endregion
    }
}