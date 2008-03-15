using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace LinFu.Delegates.Tests
{
    [TestFixture]
    public class DelegateTests
    {
        [Test]
        public void ShouldBeAbleToCreateCustomDelegate()
        {
            TestClass test = new TestClass();
            Type returnType = typeof (void);
            Type[] parameterTypes = new Type[] {typeof (string)};
            CustomDelegate methodBody = delegate(object[] args)
                                            {
                                                Console.WriteLine("Hello, {0}!", args[0]);
                                                test.TargetMethod();
                                                return null;
                                            };

            object[] arguments = new object[] {"World!"};

            Type delegateType = DelegateFactory.DefineDelegateType("__Anonymous", returnType, parameterTypes);
            MulticastDelegate target = DelegateFactory.DefineDelegate(delegateType, methodBody, returnType, parameterTypes);
            target.DynamicInvoke(arguments);

            Assert.IsTrue(test.CallCount == 1);
        }
        [Test]
        public void ShouldBindToNonStaticMethod()
        {
            MethodInfo targetMethod = typeof(TestClass).GetMethod("TargetMethod");
            TestClass instance = new TestClass();
            MulticastDelegate result = DelegateFactory.DefineDelegate(instance, targetMethod);
            result.DynamicInvoke();

            Assert.IsTrue(instance.CallCount == 1);
        }
        [Test]
        public void ShouldBindToStaticMethod()
        {
            MethodInfo targetMethod = typeof(TestClass).GetMethod("StaticTargetMethod", 
                BindingFlags.Static | BindingFlags.Public);

            MulticastDelegate result = DelegateFactory.DefineDelegate(null, targetMethod);
            result.DynamicInvoke();

            Assert.IsTrue(TestClass.StaticCallCount == 1);
        }
    }
}
