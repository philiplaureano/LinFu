using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// The attribute used to mark a property for autoinjection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, 
        AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
