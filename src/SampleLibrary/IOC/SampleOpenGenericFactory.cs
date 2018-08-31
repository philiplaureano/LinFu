using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof(ISampleGenericService<>))]
    public class SampleOpenGenericFactory : IFactory
    {
        public object CreateInstance(IFactoryRequest request)
        {
            var serviceType = request.ServiceType;
            var typeArgument = serviceType.GetGenericArguments()[0];
            var resultType = typeof(SampleGenericImplementation<>).MakeGenericType(typeArgument);

            return Activator.CreateInstance(resultType);
        }
    }
}