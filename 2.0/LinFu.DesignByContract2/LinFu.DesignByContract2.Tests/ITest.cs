using System;

namespace LinFu.DesignByContract2.Tests
{
	public interface ITest
	{
        [TestPreconditionOne]
        void DoSomething();
        void DoSomethingWithParameterPreconditions([TestParameterPrecondition] object arg1);
        object ReturnSomething();
	}
}
