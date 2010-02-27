using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a registry class that allows users to intercept fields from a single location.
    /// </summary>
    public class FieldInterceptorRegistry
    {
        private static readonly object _lock = new object();
        private static IFieldInterceptor _interceptor;

        /// <summary>
        /// Gets current the <see cref="IFieldInterceptionContext"/> associated with the <see cref="FieldInterceptorRegistry"/>.
        /// </summary>
        /// <param name="context">The <see cref="IFieldInterceptionContext"/> instance that describes the state of the method call when the field getter or setter is called.</param>
        /// <returns>The field interceptor that will be used to preempt field getter and setter calls.</returns>
        public static IFieldInterceptor GetInterceptor(IFieldInterceptionContext context)
        {
            lock (_lock)
            {
                if (_interceptor != null && _interceptor.CanIntercept(context))
                    return _interceptor;
            }

            return null;
        }

        /// <summary>
        /// Sets current the <see cref="IFieldInterceptionContext"/> that will be associated with the <see cref="FieldInterceptorRegistry"/>.
        /// </summary>
        /// <param name="interceptor">The field interceptor that will be used to preempt field getter and setter calls.</param>
        public static void SetInterceptor(IFieldInterceptor interceptor)
        {
            lock (_lock)
            {
                _interceptor = interceptor;
            }
        }
    }
}
