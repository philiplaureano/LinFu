
using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;
using NMock2;

using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;
namespace LinFu.DesignByContract2.Tests
{
	
	[TestFixture]
	public class ContractCheckerTests : BaseFixture
	{
		IContractProvider provider;
		IContractChecker checker;
		ITypeContract typeContract;
		IMethodContract methodContract;
		
		protected override void OnInit()
		{
			provider = mock.NewMock<IContractProvider>();
			
			typeContract = new TypeContract();
            methodContract = new MethodContract();
			checker = new ContractChecker(provider);
			checker.ContractProvider = provider;
            checker.Target = new object();
		}
		[Test]
		public void ShouldGetMethodContract()
		{
			Assert.IsTrue(typeof(IInvokeWrapper).IsAssignableFrom(typeof(IContractChecker)));
						
			// The checker should use the provider to 
			// retrieve a method contract for a particular item
            Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));
			Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
						
			checker.BeforeInvoke(null);
		}
		[Test]
		public void ShouldShowPreCallStateToPostconditionBeforeMethodCall()
		{
			IPostcondition postcondition = mock.NewMock<IPostcondition>();
			methodContract.Postconditions.Add(postcondition);

            Expect.AtLeastOnce.On(postcondition).Method("AppliesTo").WithAnyArguments().Will(Return.Value(true));
			Expect.AtLeastOnce.On(postcondition).Method("BeforeMethodCall").WithAnyArguments();
            Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
	
									
			checker.BeforeInvoke(null);
		}
		[Test]
		public void ShouldInvokeInvariantsBeforeAndAfterMethodCall()
		{
			IInvariant invariant = mock.NewMock<IInvariant>();
			typeContract.Invariants.Add(invariant);
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
			Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));
			
		    
			// The invariant should be called twice
            Expect.AtLeastOnce.On(invariant).Method("AppliesTo").WithAnyArguments().Will(Return.Value(true));
            //Expect.AtLeastOnce.On(invariant).Method("Check").With(Is.Null, Is.Anything, InvariantState.BeforeMethodCall).Will(Return.Value(true));
            //Expect.AtLeastOnce.On(invariant).Method("Check").With(Is.Null, Is.Anything, InvariantState.AfterMethodCall).Will(Return.Value(true));
            Expect.AtLeastOnce.On(invariant).Method("Check").With(checker.Target, null, InvariantState.BeforeMethodCall).Will(Return.Value(true));
            Expect.AtLeastOnce.On(invariant).Method("Check").With(checker.Target, null, InvariantState.AfterMethodCall).Will(Return.Value(true));
		    
			checker.BeforeInvoke(null);
			checker.AfterInvoke(null, null);						
		}
		[Test]
		[ExpectedException(typeof(InvariantViolationException))]
		public void ShouldShowInvariantViolationErrorBeforeInvoke()
		{
			IInvariant invariant = mock.NewMock<IInvariant>();
			IErrorView view = mock.NewMock<IErrorView>();
			
			typeContract.Invariants.Add(invariant);
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
            Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));
			
			// This invariant will fail and the checker should show the error
            Expect.AtLeastOnce.On(invariant).Method("AppliesTo").WithAnyArguments().Will(Return.Value(true));
			Expect.AtLeastOnce.On(invariant).Method("Check").With(checker.Target, null, InvariantState.BeforeMethodCall).Will(Return.Value(false));
			Expect.AtLeastOnce.On(invariant).Method("ShowError").WithAnyArguments();
			
			Expect.AtLeastOnce.On(view).Method("ShowInvariantError").WithAnyArguments();
			checker.ErrorView = view;
			checker.BeforeInvoke(null);
		}
		[Test]
		[ExpectedException(typeof(InvariantViolationException))]
		public void ShouldShowInvariantViolationErrorAfterInvoke()
		{
			IInvariant invariant = mock.NewMock<IInvariant>();
			IErrorView view = mock.NewMock<IErrorView>();
			
			typeContract.Invariants.Add(invariant);
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
            Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));
			
			// Allow the starting invariant to pass the check
            Expect.AtLeastOnce.On(invariant).Method("AppliesTo").WithAnyArguments().Will(Return.Value(true));
			Expect.AtLeastOnce.On(invariant).Method("Check").With(checker.Target, null, InvariantState.BeforeMethodCall).Will(Return.Value(true));
			
			// This invariant will fail and the checker should show the error
            Expect.AtLeastOnce.On(invariant).Method("Check").With(checker.Target, null, InvariantState.AfterMethodCall).Will(Return.Value(false));			
			Expect.AtLeastOnce.On(invariant).Method("ShowError").WithAnyArguments();
			
			Expect.AtLeastOnce.On(view).Method("ShowInvariantError").WithAnyArguments();
			checker.ErrorView = view;
			
			checker.BeforeInvoke(null);
			checker.AfterInvoke(null, null);
		}
		[Test]
		[ExpectedException(typeof(PreconditionViolationException))]
		public void ShouldInvokePreconditions()
		{
			IPrecondition precondition = mock.NewMock<IPrecondition>();
			IErrorView view = mock.NewMock<IErrorView>();
			
			methodContract.Preconditions.Add(precondition);
            Expect.AtLeastOnce.On(precondition).Method("AppliesTo").WithAnyArguments().Will(Return.Value(true));
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
            Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));
			Expect.AtLeastOnce.On(precondition).Method("ShowError").WithAnyArguments();
			
			// The precondition will fail and both the precondition and the view must be
			// called to display the error
			Expect.AtLeastOnce.On(precondition).Method("Check").WithAnyArguments().Will(Return.Value(false));
			Expect.AtLeastOnce.On(view).Method("ShowPreconditionError").WithAnyArguments();
			
			checker.ErrorView = view;
			checker.BeforeInvoke(null);
		}
		[Test]
		[ExpectedException(typeof(PostconditionViolationException))]
		public void ShouldInvokePostconditions()
		{
			IErrorView view = mock.NewMock<IErrorView>();
			IPostcondition postcondition = mock.NewMock<IPostcondition>();
			methodContract.Postconditions.Add(postcondition);

            Expect.AtLeastOnce.On(postcondition).Method("AppliesTo").WithAnyArguments().Will(Return.Value(true));
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
			Expect.AtLeastOnce.On(postcondition).Method("ShowError").WithAnyArguments();
			
			// The postcondition will fail and both the precondition and the view must be
			// called to display the error
			Expect.AtLeastOnce.On(postcondition).Method("Check").WithAnyArguments().Will(Return.Value(false));
			Expect.AtLeastOnce.On(view).Method("ShowPostconditionError").WithAnyArguments();
			
			checker.ErrorView = view;
			checker.AfterInvoke(null, null);
		}
	    [Test]
        public void ShouldInvokeTargetInstance()
	    {
            ITest test = mock.NewMock<ITest>();
            Expect.AtLeastOnce.On(provider).Method("GetMethodContract").WithAnyArguments().Will(Return.Value(methodContract));
            Expect.AtLeastOnce.On(provider).Method("GetTypeContract").WithAnyArguments().Will(Return.Value(typeContract));

            ProxyFactory factory = new ProxyFactory();
            
	        // Make sure that the DoSomething() method is called
            Expect.Once.On(test).Method("DoSomething").WithAnyArguments();
	        checker.Target = test;
            ITest checkedTarget = factory.CreateProxy<ITest>(checker);
            checkedTarget.DoSomething();
	    }
	}
}
