﻿using System;
using System.Linq;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Xunit;

namespace LinFu.UnitTests.AOP
{
    public class FieldInterceptionTests
    {
        public class FieldInterceptorImpl : IFieldInterceptor
        {
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
        }

        [Fact]
        public void ShouldSetAndGetTheSameFieldValue()
        {
            var myLibrary = AssemblyDefinition.ReadAssembly("SampleLibrary.dll");
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
            Assert.NotNull(instance);

            var host = (IFieldInterceptionHost) instance;
            Assert.NotNull(host);

            host.FieldInterceptor = new FieldInterceptorImpl();

            var targetProperty = targetType.GetProperty("Value");
            targetProperty?.SetValue(instance, "OtherValue", null);

            var actualValue = targetProperty?.GetValue(instance, null);

            Assert.Equal("freeze!", actualValue);
        }
    }
}