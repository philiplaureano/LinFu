using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that extends the <see cref="ParameterDefinition"/> class
    /// to emulate features found in the System.Reflection.Emit namespace.
    /// </summary>
    public static class ParameterDefinitionExtensions
    {
        /// <summary>
        /// Determines whether or not a parameter is passed by reference.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool IsByRef(this ParameterDefinition parameter)
        {
            return parameter.ParameterType != null && parameter.ParameterType is ReferenceType;
        }
    }
}
