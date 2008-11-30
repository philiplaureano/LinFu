using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Interceptors;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary.IOC
{
    [Intercepts(typeof(ISampleWrappedInterface))]
    public class SampleAroundInvokeInterceptorClass : IAroundInvoke, IInitialize, ITargetHolder
    {
        public void BeforeInvoke(IInvocationInfo info)
        {
            Target = info.Target;
        }

        public void AfterInvoke(IInvocationInfo info, object returnValue)
        {
            
        }

        public void Initialize(IServiceContainer source)
        {
            var typeName = GetType().Name;
            source.AddService<ITargetHolder>(typeName, this);
        }

        public object Target
        {
            get;
            set;
        }
    }
}
