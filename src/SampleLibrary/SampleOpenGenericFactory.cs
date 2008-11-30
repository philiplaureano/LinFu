using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof(ISampleGenericService<>))]
    public class SampleOpenGenericFactory : IFactory
    {
        #region IFactory Members

        public object CreateInstance(Type serviceType, IContainer container)
        {
            Type typeArgument = serviceType.GetGenericArguments()[0];
            Type actualType = typeof(SampleGenericImplementation<>).MakeGenericType(typeArgument);

            return Activator.CreateInstance(actualType);
        }

        #endregion
    }
}
