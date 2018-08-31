using System;
using System.Collections.Generic;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    ///     Represents a class that keeps track of the internal object instances that should be ignored
    ///     by the interception routines.
    /// </summary>
    public static class IgnoredInstancesRegistry
    {
        private static readonly HashSet<int> _instances;
        private static readonly object _lock = new object();

        static IgnoredInstancesRegistry()
        {
            _instances = new HashSet<int>();
        }

        /// <summary>
        ///     Determines whether or not the registry contains the given ignored object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>Returns <c>true</c> if the object should be ignored; otherwise, it will return <c>false</c>.</returns>
        public static bool Contains(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var hash = target.GetHashCode();

            return _instances.Contains(hash);
        }

        /// <summary>
        ///     Adds an instance to the list of ignored instances.
        /// </summary>
        /// <param name="target">The target instance to be ignored by the interception routines.</param>
        public static void AddInstance(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            lock (_lock)
            {
                var hash = target.GetHashCode();
                _instances.Add(hash);
            }
        }
    }
}