using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Injectors;
using LinFu.DynamicProxy;
using NMock2;
using NUnit.Framework;

namespace LinFu.DesignByContract2.Tests.Fixtures
{
    [TestFixture]
    public class PervasiveInjectorTests : BaseFixture
    {
        protected override void OnInit()
        {
            // TODO: Insert your initialization code here
        }

        protected override void OnTerm()
        {
            // TODO: Insert your cleanup code here
        }

        [Test]
        public void ShouldCallInnerWrapper()
        {
            IInvokeWrapper wrapper = mock.NewMock<IInvokeWrapper>();
            IInvokeWrapper injector = new PervasiveWrapper(wrapper);

            Expect.Once.On(wrapper).Method("BeforeInvoke").WithAnyArguments();
            Expect.Once.On(wrapper).Method("AfterInvoke").WithAnyArguments();
            Expect.Once.On(wrapper).Method("DoInvoke").WithAnyArguments().Will(Return.Value(null));
            
            injector.BeforeInvoke(null);
            injector.DoInvoke(null);
            injector.AfterInvoke(null, null);
        }        
    }   
}
