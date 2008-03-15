using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public class TestPreconditionOneAttribute : BaseTestPreconditionAttribute
    {
        
    }
}
