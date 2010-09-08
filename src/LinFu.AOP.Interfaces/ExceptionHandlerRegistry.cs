using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that stores <see cref="IExceptionHandler"/> instances in a central location.
    /// </summary>
    public static class ExceptionHandlerRegistry
    {
        private static IExceptionHandler _handler;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the <see cref="IExceptionHandler"/> instance that can handle the current exception.
        /// </summary>
        /// <param name="info">The <see cref="IExceptionHandlerInfo"/> instance that describes the context of the thrown exception.</param>
        /// <returns>An exception handler.</returns>
        public static IExceptionHandler GetHandler(IExceptionHandlerInfo info)
        {
            if (_handler == null)
                return null;

            if (!_handler.CanCatch(info))
                return null;

            return _handler;
        }

        /// <summary>
        /// Sets the <see cref="IExceptionHandler"/> instance that can handle all thrown exceptions.
        /// </summary>
        /// <param name="handler">The exception handler.</param>
        public static void SetHandler(IExceptionHandler handler)
        {
            lock (_lock)
            {
                _handler = handler;
            }
        }
    }
}
