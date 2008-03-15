using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestPreconditionTwoAttribute : BaseTestPreconditionAttribute
    {
    }
}
