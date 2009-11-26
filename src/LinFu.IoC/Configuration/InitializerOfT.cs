using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that initializes service instances that use
    /// the <see cref="IInitialize{T}"/> interface.
    /// </summary>
    public class Initializer<T> : IPostProcessor
    {
        private static readonly HashSet<HashableWeakReference> _instances = new HashSet<HashableWeakReference>(new HashableWeakReferenceComparer());
        private readonly Func<IServiceRequestResult, T> _getSource;
        static private int _initializeCallCount;
        #region IPostProcessor Members

        /// <summary>
        /// Initializes the class with the given <paramref name="getSource"/> delegate.
        /// </summary>
        /// <param name="getSource">The functor that will obtain the object instance that will be used to initialize a given service.</param>
        public Initializer(Func<IServiceRequestResult, T> getSource)
        {
            _getSource = getSource;
        }

        /// <summary>
        /// Initializes every service that implements
        /// the <see cref="IInitialize{T}"/> interface.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> instance that contains the service instance to be initialized.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            var originalResult = result.OriginalResult as IInitialize<T>;
            var actualResult = result.ActualResult as IInitialize<T>;

            var source = _getSource(result);

            // Initialize the original result, if possible
            Initialize(originalResult, source);

            // Initialize the actual result
            Initialize(actualResult, source);
        }
        
        /// <summary>
        /// Initializes the <paramref name="target"/> with the given <paramref name="source"/> instance.
        /// </summary>
        /// <param name="target">The target to initialize.</param>
        /// <param name="source">The instance that will be introduced to the <see cref="IInitialize{T}"/> instance.</param>        
        private static void Initialize(IInitialize<T> target, T source)
        {
            if (target == null)
                return;

            if ((_initializeCallCount = ++_initializeCallCount % 100) == 0)
                _instances.RemoveWhere(w => !w.IsAlive);

            // Make sure that the target is initialized only once
            var weakReference = new HashableWeakReference(target);
            if (_instances.Contains(weakReference))
                return;

            // Initialize the target
            target.Initialize(source);
            _instances.Add(weakReference);
        }

        #endregion

        private class HashableWeakReferenceComparer : IEqualityComparer<HashableWeakReference>
        {
            public bool Equals(HashableWeakReference x, HashableWeakReference y)
            {
                return x.Target == y.Target;
            }

            int IEqualityComparer<HashableWeakReference>.GetHashCode(HashableWeakReference obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }
                return obj.GetHashCode();
            }
        }

        private class HashableWeakReference : WeakReference
        {
            private readonly int _hashCode;

            public HashableWeakReference(object target) : base(target, false)
            {
                _hashCode = target.GetHashCode();
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}
