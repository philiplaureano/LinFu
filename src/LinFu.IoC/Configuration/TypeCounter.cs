using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Counts the number of occurrences of a specific type.
    /// </summary>
    internal class TypeCounter
    {
        private readonly Dictionary<int, Dictionary<Type, int>> _counts = new Dictionary<int, Dictionary<Type, int>>();

        /// <summary>
        /// Increments the count for the current <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type being counted.</param>
        public void Increment(Type type)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            // Create a new counter, if necessary
            if (!_counts.ContainsKey(threadId))
            {
                lock (_counts)
                {
                    _counts[threadId] = new Dictionary<Type, int>();
                }
            }

            var currentCounts = _counts[threadId];
            if (!currentCounts.ContainsKey(type))
            {
                lock (currentCounts)
                {
                    currentCounts[type] = 0;
                }
            }
       
            lock (currentCounts)
            {
                currentCounts[type]++;
            }
        }

        /// <summary>
        /// Returns the number of occurrences of a specific <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type being counted.</param>
        /// <returns>The number of occurrences for the given type.</returns>
        public int CountOf(Type type)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            if (!_counts.ContainsKey(threadId))
                return 0;

            var currentCounts = _counts[threadId];
            return currentCounts[type];
        }

        /// <summary>
        /// Decrements the count for the current <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type being counted.</param>
        public void Decrement(Type type)
        {
            var currentCount = CountOf(type);
            if (currentCount > 0)
                currentCount--;

            var threadId = Thread.CurrentThread.ManagedThreadId;

            // Create a new counter, if necessary
            if (!_counts.ContainsKey(threadId))
            {
                lock (_counts)
                {
                    _counts[threadId] = new Dictionary<Type, int>();
                }
            }

            // Split the counts by thread
            var currentCounts = _counts[threadId];
            lock (currentCounts)
            {
                currentCounts[type] = currentCount;
            }
        }

        /// <summary>
        /// Gets the value indicating the types that are
        /// currently being counted.
        /// </summary>
        public IEnumerable<Type> AvailableTypes
        {
            get
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                if (!_counts.ContainsKey(threadId))
                    return new Type[0];

                var results = new List<Type>();
                foreach (var type in _counts[threadId].Keys)
                {
                    results.Add(type);
                }
                
                return results;
            }
        }

        /// <summary>
        /// Resets the counts back to zero.
        /// </summary>
        public void Reset()
        {
            _counts.Clear();
        }
    }
}
