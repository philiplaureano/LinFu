using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using LinFu.AOP;
using LinFu.AOP.Cecil;
using LinFu.Proxy;
using LinFu.IoC.Reflection;
using LinFu.Reflection.Emit;
using LinFu.AOP.Interfaces;
using Moq;
using System.Reflection;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class ThirdPartyMethodCallInterceptionTests : BaseTestFixture
    {
        [Test]
        public void ShouldImplementIMethodReplacementHostOnTargetType()
        {
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = assembly.MainModule;

            var typeName = "SampleClassWithThirdPartyMethodCall";
            var targetType = (from TypeDefinition t in module.Types
                              where t.Name == typeName
                              select t).First();

            // Intercept all calls to the System.Console.WriteLine method from the DoSomething method
            targetType.InterceptMethodCalls(t => t.Name.Contains(typeName), m => m.DeclaringType.Name.Contains(typeName) && m.Name == "DoSomething",
                methodCall => methodCall.DeclaringType.Name == "Console" && methodCall.Name == "WriteLine");

            var modifiedAssembly = assembly.ToAssembly();
            var modifiedTargetType = (from t in modifiedAssembly.GetTypes()
                                      where t.Name == typeName
                                      select t).First();

            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is IMethodReplacementHost);

            IMethodReplacementHost host = (IMethodReplacementHost)instance;

            var interceptor = new SampleMethodReplacement();

            host.MethodReplacementProvider = new SampleMethodReplacementProvider(interceptor);

            MethodInfo targetMethod = modifiedTargetType.GetMethod("DoSomething");
            try
            {
                targetMethod.Invoke(instance, null);
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException;
                Console.WriteLine(ex.ToString());
                throw innerException;
            }

            Assert.IsTrue(interceptor.HasBeenCalled);
        }
    }
}
