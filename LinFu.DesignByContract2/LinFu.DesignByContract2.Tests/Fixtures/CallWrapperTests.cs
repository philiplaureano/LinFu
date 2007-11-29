using System;
using System.Reflection;

using NMock2;
using NUnit.Framework;

using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Tests
{
	[TestFixture]
	public class CallWrapperTests
	{
		Mockery mock;
		[SetUp]
		public void Init()
		{
			mock = new Mockery();
		}
		[TearDown]
		public void Term()
		{
			mock.VerifyAllExpectationsHaveBeenMet();
		}
		[Test]
		public void ShouldCallWrapperDuringMethodCall()
		{
			IInvokeWrapper wrapper = mock.NewMock<IInvokeWrapper>();
			
			// The interceptor should call the wrapper once the method
			// is invoked
			Expect.Once.On(wrapper).Method("BeforeInvoke").WithAnyArguments();
			Expect.Once.On(wrapper).Method("DoInvoke").WithAnyArguments().Will(Return.Value(null));
			Expect.Once.On(wrapper).Method("AfterInvoke").WithAnyArguments();
			
			ProxyFactory factory = new ProxyFactory();
			ITest test = factory.CreateProxy<ITest>(wrapper);
			
			test.DoSomething();				
		}
	}
}