using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using NUnit.Framework;

namespace LinFu.DesignByContract2.Tests
{
    [TestFixture]
    public class MethodFinderPredicateTests : BaseFixture 
    {
        private MethodInfo targetMethod;
        private PredicateBuilder builder;
        protected override void OnInit()
        {
            targetMethod = typeof(MethodFinderTargetDummy).GetMethod("DoSomething");
            builder = new PredicateBuilder();
        }
        protected override void OnTerm()
        {
            builder = null;
        }
        [Test]
        public void ShouldMatchMethodName()
        {            
            builder.MethodName = "DoSomething";
            RunTest();
        }

        [Test]
        public void ShouldMatchReturnType()
        {
            builder.ReturnType = targetMethod.ReturnType;
            RunTest();
        }        
        
        [Test]
        public void ShouldMatchParameterTypes()
        {
            builder.SetParameterTypes(targetMethod.GetParameters());
            RunTest();
        }

        [Test]
        public void ShouldMatchGenericParameters()
        {
            targetMethod = typeof (MethodFinderTargetDummy).GetMethod("DoSomethingGeneric");
            Assert.IsTrue(targetMethod.IsGenericMethodDefinition);

            Type typeArgument = typeof (int);
            targetMethod = targetMethod.MakeGenericMethod(typeArgument);

            builder.TypeArguments.Add(typeArgument);
            RunTest();
        }

        [Test]
        public void ShouldMatchPublicMethod()
        {
            builder.IsPublic = true;            
            RunTest();                        
        }

        [Test]
        public void ShouldMatchProtectedMethod()
        {
            builder.IsProtected = false;
            RunTest();
        }
        
        [Test]
        [Ignore("TODO: Implement this in a future version")]
        public void ShouldMatchBasedOnRuntimeArguments()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore("TODO: Implement this in a future version")]
        public void ShouldMatchCovariantReturnType()
        {
            throw new NotImplementedException();
        }
        private void RunTest()
        {
            Predicate<MethodInfo> predicate = builder.CreatePredicate();
            Assert.IsNotNull(predicate);
            
            foreach(Predicate<MethodInfo> current in predicate.GetInvocationList())
            {
                Assert.IsTrue(current(targetMethod));
            }
        }
    }
}
