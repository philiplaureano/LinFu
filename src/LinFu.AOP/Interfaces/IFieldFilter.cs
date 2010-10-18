using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that determines whether or not a particular field get or set should be intercepted.
    /// </summary>
    public interface IFieldFilter
    {
        /// <summary>
        /// Determines whether or not a particular field get or set should be intercepted.
        /// </summary>
        /// <param name="hostMethod">The host method.</param>
        /// <param name="targetField">The target field.</param>
        /// <returns>Returns <c>true</c> if the field should be intercepted; otherwise, it will return <c>false</c>.</returns>
        bool ShouldWeave(MethodReference hostMethod, FieldReference targetField);
    }
}