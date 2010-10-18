using LinFu.AOP.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interceptors;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Intercepts(typeof (ISampleWrappedInterface))]
    public class SampleAroundInvokeInterceptorClass : IAroundInvoke, IInitialize, ITargetHolder
    {
        #region IAroundInvoke Members

        public void BeforeInvoke(IInvocationInfo info)
        {
            Target = info.Target;
        }

        public void AfterInvoke(IInvocationInfo info, object returnValue)
        {
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IServiceContainer source)
        {
            string typeName = GetType().Name;
            source.AddService<ITargetHolder>(typeName, this);
        }

        #endregion

        #region ITargetHolder Members

        public object Target { get; set; }

        #endregion
    }
}