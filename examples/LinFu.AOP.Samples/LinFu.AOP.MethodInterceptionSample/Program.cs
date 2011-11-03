using System;
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

            // Replace the input parameter with 42
            var result = info.TargetMethod.Invoke(info.Target, new object[]{42});

            return result;
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
                modifiableType.MethodBodyReplacementProvider = new SimpleMethodReplacementProvider(new SampleInterceptor());

            // The employee object will call the interceptor instead of the actual method implementation
            employee.Pay(12345);

            return;
        }
    }
}
