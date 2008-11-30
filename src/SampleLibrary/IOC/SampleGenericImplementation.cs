using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    public class SampleGenericImplementation<T> : ISampleGenericService<T>, IInitialize
    {
        public void Initialize(IServiceContainer source)
        {
            Called = true;
        }

        public bool Called
        {
            get; private set;
        }
    }
}