using LinFu.AOP.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interceptors;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Intercepts(typeof(ISampleInterceptedInterface))]
    public class SampleInterceptorClass : IInterceptor, IInitialize, ITargetHolder
    {
        public void Initialize(IServiceContainer source)
        {
            var typeName = GetType().Name;
            source.AddService<ITargetHolder>(typeName, this);
        }


        public object Intercept(IInvocationInfo info)
        {
            // Set the target on every method call
            Target = info.Target;
            return null;
        }


        public object Target { get; set; }
    }
}