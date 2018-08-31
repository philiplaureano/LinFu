namespace LinFu.AOP.Interfaces
{
    /// <summary>
    ///     Represents a type that has been modified to support
    ///     pervasive method interception.
    /// </summary>
    public interface IModifiableType : IMethodReplacementHost, IAroundInvokeHost
    {
        /// <summary>
        ///     Gets or sets the value indicating whether or not
        ///     method interception should be disabled.
        /// </summary>
        bool IsInterceptionDisabled { get; set; }
    }
}