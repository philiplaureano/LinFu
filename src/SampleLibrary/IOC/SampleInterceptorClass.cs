using LinFu.AOP.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interceptors;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Intercepts(typeof (ISampleInterceptedInterface))]
    public class SampleInterceptorClass : IInterceptor, IInitialize, ITargetHolder
    {
        #region IInitialize Members

        public void Initialize(IServiceContainer source)
        {
            string typeName = GetType().Name;
            source.AddService<ITargetHolder>(typeName, this);
        }

        #endregion

        #region IInterceptor Members

        public object Intercept(IInvocationInfo info)
        {
            // Set the target on every method call
            Target = info.Target;
            return null;
        }

        #endregion

        #region ITargetHolder Members

        public object Target { get; set; }

        #endregion
    }
}