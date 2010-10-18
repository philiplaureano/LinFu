using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    public class SampleGenericImplementation<T> : ISampleGenericService<T>, IInitialize
    {
        #region IInitialize Members

        public void Initialize(IServiceContainer source)
        {
            Called = true;
        }

        #endregion

        #region ISampleGenericService<T> Members

        public bool Called { get; private set; }

        #endregion
    }
}