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

        public static IExceptionHandler GetHandler(IExceptionHandlerInfo info)
        {
            if (_handler == null)
                return null;

            if (!_handler.CanCatch(info))
                return null;

            return _handler;
        }

        public static void SetHandler(IExceptionHandler handler)
        {
            lock (_lock)
            {
                _handler = handler;
            }
        }
    }
}
