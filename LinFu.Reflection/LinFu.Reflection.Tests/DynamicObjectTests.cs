using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;
using NMock2;
using NUnit.Framework;

namespace LinFu.Reflection.Tests
{
    [TestFixture]
    public class DynamicObjectTests : BaseFixture 
    {
        [Test]
        public void ShouldCallTargetMethod()
        {
            string methodName = "TargetMethod";
            MockClass mockTarget = new MockClass();
            
            DynamicObject dynamic = new DynamicObject(mockTarget);
            dynamic.Methods[methodName]();
            Assert.AreEqual(1, mockTarget.CallCount, "The target method was not called!");            
        }

        [Test]
        public void ShouldCallTargetProperty()
        {
            string propertyName = "TargetProperty";
            MockClass mockTarget = new MockClass();

            DynamicObject dynamic = new DynamicObject(mockTarget);

            // Call the getter and the setter
            dynamic.Properties[propertyName] = 0;
            object value = dynamic.Properties[propertyName];

            Assert.AreEqual(2, mockTarget.CallCount, "The target property was not called!");            
        }

        [Test]
        public void ShouldBeAbleToAddNewMethod()
        {
            IntegerOperation addBody = delegate(int a, int b) { return a + b; };
            DynamicObject dynamic = new DynamicObject(new object());
            dynamic.AddMethod("Add", addBody);

            int result = (int) dynamic.Methods["Add"](1, 1);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void ShouldBeAbleToMixAnotherClassInstance()
        {
            MockClass test = new MockClass();
            DynamicObject dynamic = new DynamicObject(new object());

            string methodName = "TargetMethod";
            dynamic.MixWith(test);
            dynamic.Methods[methodName]();
            Assert.AreEqual(1, test.CallCount);
        }
        [Test]
        public void ShouldAssignSelfToMixinAwareInstance()
        {
            IMixinAware test = mock.NewMock<IMixinAware>();            
            DynamicObject dynamic = new DynamicObject(new object());
            Expect.Once.On(test).SetProperty("Self").To(dynamic);

            dynamic.MixWith(test);
        }

        [Test]
        public void ShouldAllowDuckTyping()
        {
            MockClass test = new MockClass();
            DynamicObject dynamic = new DynamicObject(new object());

            ITest duck = dynamic.CreateDuck<ITest>();

            // Combine the MockClass implementation with the current
            // object instance
            dynamic.MixWith(test);
            duck.TargetMethod();
            duck.TargetMethod<int>();
            Assert.AreEqual(2, test.CallCount);
        }

        [Test]
        public void ShouldBeAbleToCombineMultipleDynamicObjects()
        {
            FirstClass firstInstance = new FirstClass();
            SecondClass secondInstance = new SecondClass();
            DynamicObject first = new DynamicObject(firstInstance);
            DynamicObject second = new DynamicObject(secondInstance);

            DynamicObject combined = first + second;
            combined.Methods["TestMethod1"]();
            combined.Methods["TestMethod2"]();

            Assert.AreEqual(1, firstInstance.CallCount);
            Assert.AreEqual(1, secondInstance.CallCount);
        }
        [Test]
        public void ShouldBeAbleToTellWhetherOrNotSomethingLooksLikeADuck()
        {
            DynamicObject dynamic = new DynamicObject(new RubberDucky());
            Assert.IsTrue(dynamic.LooksLike(typeof (IDuck)));
            Assert.IsTrue(dynamic.LooksLike<IDuck>());
        }

        [Test]
        public void ShouldBeAbleToAddMethodsUsingRuntimeAnonymousDelegates()
        {
            CustomDelegate addBody = delegate(object[] args)
                                         {
                                             int a = (int)args[0];
                                             int b = (int) args[1];
                                             return a + b;
                                         };

            DynamicObject dynamic = new DynamicObject(new object());
            Type returnType = typeof (int);
            Type[] parameterTypes = new Type[] {typeof (int), typeof (int)};
            dynamic.AddMethod("Add", addBody, returnType, parameterTypes);

            int result = (int)dynamic.Methods["Add"](1, 1);
            Assert.AreEqual(2, result);
        }
    }
}
