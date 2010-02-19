using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{   
    /// <summary>
    /// Represents a type that can catch thrown exceptions.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Determines whether or not an exception can be handled.
        /// </summary>
        /// <param name="exceptionHandlerInfo">The object that describes the exception being thrown.</param>
        /// <returns><c>True</c> if the exception can be handled by the current handler.</returns>
        bool CanCatch(IExceptionHandlerInfo exceptionHandlerInfo);

        /// <summary>
        /// Handles the exception specified in the <paramref name="exceptionHandlerInfo"/> instance.
        /// </summary>
        /// <param name="exceptionHandlerInfo">The object that describes the exception being thrown.</param>
        void Catch(IExceptionHandlerInfo exceptionHandlerInfo);       
    }
}
