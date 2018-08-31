using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    ///     Represents a type that determines the method calls that need to be intercepted.
    /// </summary>
    public interface IMethodCallFilter
    {
        /// <summary>
        ///     Determines whether or not a particular method call should be intercepted.
        /// </summary>
        /// <param name="targetType">The host type that contains the method call.</param>
        /// <param name="hostMethod">The method that contains the current method call.</param>
        /// <param name="currentMethodCall">The method call to be intercepted.</param>
        /// <returns>Returns <c>true</c> if the method call should be intercepted; otherwise, it will return <c>false</c>.</returns>
        bool ShouldWeave(TypeReference targetType, MethodReference hostMethod, MethodReference currentMethodCall);
    }
}