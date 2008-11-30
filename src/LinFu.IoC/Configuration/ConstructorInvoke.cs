using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that invokes constructor instances.
    /// </summary>
    public class ConstructorInvoke : BaseMethodInvoke<ConstructorInfo>
    {
        /// <summary>
        /// Initializes the class with the default values.
        /// </summary>
        public ConstructorInvoke()
        {
            MethodBuilder = new ReflectionMethodBuilder<ConstructorInfo>();
        }
    }
}
