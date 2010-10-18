using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using SampleLibrary;

namespace LinFu.AOP.MethodInterceptionSample
{
    public class SampleInterceptor : IInterceptor
    {
        public object Intercept(IInvocationInfo info)
        {
            var methodName = info.TargetMethod.Name;
            Console.WriteLine("method '{0}' called", methodName);
            return null;
        }
    }
    public class SampleMethodReplacementProvider : IMethodReplacementProvider
    {
        public bool CanReplace(object host, IInvocationInfo info)
        {
            var employee = host as Employee;

            return employee != null;
        }

        public IInterceptor GetMethodReplacement(object host, IInvocationInfo info)
        {
            return new SampleInterceptor();
        }
    }

    // NOTE: Remember to check the SampleLibrary.csproj file for an example on how to use LinFu.AOP.Tasks to transparently
    // postweave your DLLs!
    class Program
    {
        static void Main(string[] args)
        {
            var employee = new Employee();

            // All LinFu.AOP-modified objects implement the IModifiableType interface
            var modifiableType = employee as IModifiableType;

            // Plug in our custom implementation
            if (modifiableType != null)
                modifiableType.MethodBodyReplacementProvider = new SampleMethodReplacementProvider();

            // The employee object will call the interceptor instead of the actual method implementation
            employee.Pay(12345);

            return;
        }
    }
}
