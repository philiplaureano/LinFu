using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that describes the context of a thrown exception.
    /// </summary>
    public class ExceptionHandlerInfo : IExceptionHandlerInfo
    {
        private readonly Exception _ex;
        private readonly IInvocationInfo _invocationInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerInfo"/> class.
        /// </summary>
        /// <param name="ex">The thrown exception.</param>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> instance that describes the context of the method call.</param>
        public ExceptionHandlerInfo(Exception ex, IInvocationInfo invocationInfo)
        {
            _ex = ex;
            _invocationInfo = invocationInfo;
        }

        /// <summary>
        /// Gets the value indicating the thrown exception.
        /// </summary>
        /// <value>The thrown exception.</value>
        public Exception Exception
        {
            get { return _ex; }
        }

        /// <summary>
        /// Gets the value indicating the <see cref="IInvocationInfo"/> instance that describes the context of the method
        /// that threw the exception.
        /// </summary>
        /// <value>The <see cref="IInvocationInfo"/> instance.</value>
        public IInvocationInfo InvocationInfo
        {
            get { return _invocationInfo; }
        }

        /// <summary>
        /// Gets or sets the value indicating the return value that will be used in place of the original return value if 
        /// the exception is intercepted by an <see cref="IExceptionHandler"/> instance.
        /// </summary>
        /// <value>The method return value.</value>
        public object ReturnValue { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not the exception should be rethrown after
        /// the <see cref="IExceptionHandler"/> handles the given exception.
        /// </summary>
        /// <value>This should be <c>true</c> if the exception should be rethrown, otherwise, it must be <c>false</c>.</value>
        public bool ShouldSkipRethrow { get; set; }
    }
}