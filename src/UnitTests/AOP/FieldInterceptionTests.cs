using System;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using NUnit.Framework;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class FieldInterceptionTests
    {
        public class FieldInterceptorImpl : IFieldInterceptor
        {
            #region IFieldInterceptor Members

            public bool CanIntercept(IFieldInterceptionContext context)
            {
                return true;
            }

            public object GetValue(IFieldInterceptionContext context)
            {
                return "freeze!";
            }

            public object SetValue(IFieldInterceptionContext context, object value)
            {
                // Prevent any setter calls by replacing the setter value
                return "freeze!";
            }

            #endregion
        }

        [Test]
        public void ShouldSetAndGetTheSameFieldValue()
        {
            var myLibrary = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = myLibrary.MainModule;

            foreach (TypeDefinition type in myLibrary.MainModule.Types)
            {
                if (!type.FullName.Contains("SampleClassWithReadOnlyField"))
                    continue;

                type.InterceptAllFields();
            }

            var loadedAssembly = myLibrary.ToAssembly();
            var targetType = (from t in loadedAssembly.GetTypes()
                               where t.Name.Contains("SampleClassWithReadOnlyField")
                               select t).First();

            var instance = Activator.CreateInstance(targetType);
            Assert.IsNotNull(instance);

            var host = (IFieldInterceptionHost) instance;
            Assert.IsNotNull(host);

            host.FieldInterceptor = new FieldInterceptorImpl();

            var targetProperty = targetType.GetProperty("Value");
            targetProperty.SetValue(instance, "OtherValue", null);

            var actualValue = targetProperty.GetValue(instance, null);

            Assert.AreEqual("freeze!", actualValue);
        }
    }
}