using System;
using System.Reflection;

using NMock2;
using NUnit.Framework;

namespace LinFu.DesignByContract2.Tests
{
	[TestFixture]
	public class BaseFixture
	{
		protected Mockery mock;
		[SetUp]
		public void Init()
		{
			mock = new Mockery();
			
			OnInit();
		}
		[TearDown]
		public void Term()
		{
			mock.VerifyAllExpectationsHaveBeenMet();
			
			OnTerm();
		}
		
		protected virtual void OnInit() {}
		protected virtual void OnTerm() {}
	}
}
