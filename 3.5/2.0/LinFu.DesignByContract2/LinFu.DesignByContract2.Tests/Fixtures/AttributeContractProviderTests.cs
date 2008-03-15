using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;
using NMock2;
using NUnit.Framework;

namespace LinFu.DesignByContract2.Tests
{
    [TestFixture]
    public class AttributeContractProviderTests : BaseFixture
    {
        [Test]
        public void ShouldScanMethodForPreconditions()
        {
            IContractProvider provider = new AttributeContractProvider();

            // Mock up the invocation info to point to the test interface
            MethodInfo targetMethod = typeof (ITest).GetMethod("DoSomething");
            InvocationInfo info = new InvocationInfo(null, targetMethod, null, new Type[0], new object[0]);

            Type targetType = typeof (ITest);
            // This should return the TestPreconditionAttribute
            IMethodContract contract = provider.GetMethodContract(targetType, info);
            Assert.IsNotNull(contract, "The resulting contract cannot be null");
            Assert.IsTrue(contract.Preconditions.Count > 0, "No Preconditions Found");
            Assert.IsInstanceOfType(typeof (TestPreconditionOneAttribute), contract.Preconditions[0]);            
        }

        [Test]
        public void ShouldScanMethodForParameterPreconditions()
        {
            IContractProvider provider = new AttributeContractProvider();

            // Mock up the invocation info to point to the test interface
            MethodInfo targetMethod = typeof(ITest).GetMethod("DoSomethingWithParameterPreconditions");
            InvocationInfo info = new InvocationInfo(null, targetMethod, null, new Type[0], new object[0]);

            Type targetType = typeof (ITest);                        
            IMethodContract contract = provider.GetMethodContract(targetType, info);
            Assert.IsNotNull(contract, "The resulting contract cannot be null");
            Assert.IsTrue(contract.Preconditions.Count > 0, "No Preconditions Found");            
        }
        [Test]
        public void ShouldInheritPreconditionFromParent()
        {
            IContractProvider provider = new AttributeContractProvider();
            MethodInfo targetMethod = typeof(TestChild1).GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);
            InvocationInfo info = new InvocationInfo(null, targetMethod, null, new Type[0], new object[0]);

            Type targetType = typeof (TestChild1);
            IMethodContract contract = provider.GetMethodContract(targetType, info);
            Assert.IsNotNull(contract, "The resulting contract cannot be null");
            Assert.IsTrue(contract.Preconditions.Count > 0, "No Preconditions Found");
            Assert.IsInstanceOfType(typeof(TestPreconditionOneAttribute), contract.Preconditions[0]);                        
        }

        [Test]
        public void ShouldScanForInvariants()
        {
            IContractProvider provider = new AttributeContractProvider();
            ITypeContract contract = provider.GetTypeContract(typeof (IInvariantTest));
            
            Assert.IsNotNull(contract);
            Assert.IsTrue(contract.Invariants.Count > 0);
            Assert.IsInstanceOfType(typeof (TestInvariantAttribute), contract.Invariants[0]);
        }
    }
}