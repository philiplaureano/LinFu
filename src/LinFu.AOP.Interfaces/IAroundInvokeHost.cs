namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that supports injecting code around a method body or method call.
    /// </summary>
    public interface IAroundInvokeHost
    {
        /// <summary>
        /// Gets or sets the value indicating the <see cref="IAroundInvokeProvider"/>
        /// that will be used to inject code "around" a particular method body
        /// implementation.
        /// </summary>
        IAroundInvokeProvider AroundMethodBodyProvider { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IAroundInvokeProvider"/>
        /// that will be used to inject code "around" a particular method call
        /// implementation.
        /// </summary>
        IAroundInvokeProvider AroundMethodCallProvider { get; set; }
    }
}