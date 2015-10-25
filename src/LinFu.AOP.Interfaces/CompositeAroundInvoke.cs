using System;
using System.Collections.Generic;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that can wrap itself around any given method call.
    /// </summary>
    public class CompositeAroundInvoke : IAroundInvoke
    {
        private readonly IList<IAroundInvoke> _aroundInvokeList = new List<IAroundInvoke>();

        /// <summary>
        /// Constructs a composit around invoke.
        /// </summary>
        /// <param name="aroundInvokeList">An collection of <see cref="IAroundInvoke"/> to </param>
        public CompositeAroundInvoke(IEnumerable<IAroundInvoke> aroundInvokeList)
        {
            if (aroundInvokeList == null)
                throw new ArgumentNullException("aroundInvokeList");

            // Filter out the null values
            foreach (var current in aroundInvokeList)
            {
                if (current == null)
                    continue;

                _aroundInvokeList.Add(current);
            }
        }

        /// <summary>
        /// This method will be called immediately after the actual method call is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <param name="returnValue">The value returned from the actual method call.</param>
        public void AfterInvoke(IInvocationInfo context, object returnValue)
        {
            foreach (var invoke in _aroundInvokeList)
            {
                invoke.AfterInvoke(context, returnValue);
            }
        }

        /// <summary>
        /// This method will be called just before the actual method call is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <seealso cref="IInvocationInfo"/>
        public void BeforeInvoke(IInvocationInfo context)
        {
            foreach (var invoke in _aroundInvokeList)
            {
                invoke.BeforeInvoke(context);
            }
        }
    }
}