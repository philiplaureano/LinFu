using System;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that describes the context of a thrown exception.
    /// </summary>
    public interface IExceptionHandlerInfo
    {
        /// <summary>
        /// Gets the value indicating the thrown exception.
        /// </summary>
        /// <value>The thrown exception.</value>
        Exception Exception { get; }

        /// <summary>
        /// Gets the value indicating the <see cref="IInvocationInfo"/> instance that describes the context of the method
        /// that threw the exception.
        /// </summary>
        /// <value>The <see cref="IInvocationInfo"/> instance.</value>
        IInvocationInfo InvocationInfo { get; }

        /// <summary>
        /// Gets or sets the value indicating the return value that will be used in place of the original return value if 
        /// the exception is intercepted by an <see cref="IExceptionHandler"/> instance.
        /// </summary>
        /// <value>The method return value.</value>
        object ReturnValue { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not the exception should be rethrown after
        /// the <see cref="IExceptionHandler"/> handles the given exception.
        /// </summary>
        /// <value>This should be <c>true</c> if the exception should be rethrown, otherwise, it must be <c>false</c>.</value>
        bool ShouldSkipRethrow { get; set; }
    }
}