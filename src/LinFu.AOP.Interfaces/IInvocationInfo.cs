using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents the information associated with 
    /// a single method call.
    /// </summary>
    public interface IInvocationInfo
    {
        /// <summary>
        /// The target instance currently being called.
        /// </summary>
        /// <remarks>This typically is a reference to a proxy object.</remarks>
        object Target { get; }

        /// <summary>
        /// The method currently being called.
        /// </summary>
        MethodInfo TargetMethod { get; }

        /// <summary>
        /// The return type of the <see cref="TargetMethod"/>.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// The parameter types for the current target method.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This could be very useful in cases where the actual target method
        /// is based on a generic type definition. In such cases, 
        /// the <see cref="IInvocationInfo"/> instance needs to be able
        /// to describe the actual parameter types being used by the
        /// current generic type instantiation. This property helps
        /// users determine which parameter types are actually being used
        /// at the time of the method call.
        /// </para>
        /// </remarks>
        Type[] ParameterTypes { get; }

        /// <summary>
        /// If the <see cref="TargetMethod"/> method is a generic method, 
        /// this will hold the generic type arguments used to construct the
        /// method.
        /// </summary>
        Type[] TypeArguments { get; }

        /// <summary>
        /// The arguments used in the method call.
        /// </summary>
        object[] Arguments { get; }        
    }
}