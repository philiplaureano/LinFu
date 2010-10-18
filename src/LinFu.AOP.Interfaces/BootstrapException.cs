using System;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents an exception thrown when LinFu.AOP is unable to bootstrap itself.
    /// </summary>
    [Serializable]
    public class BootstrapException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="ex">The exception itself.</param>
        public BootstrapException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}