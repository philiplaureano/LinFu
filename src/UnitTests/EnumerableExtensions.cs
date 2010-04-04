using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.UnitTests
{
    public static class EnumerableExtensions
    {
        public static bool AreEqualTo<T>(this IEnumerable<T> expectedArray, IEnumerable<T> actualArray)
        {
            var areEqual = true;

            var expectedItems = new Queue<T>(expectedArray);
            var actualItems = new Queue<T>(actualArray);

            if (expectedItems.Count != actualItems.Count)
                return false;

            while (expectedItems.Count > 0)
            {
                var expectedItem = expectedItems.Dequeue();
                var actualItem = actualItems.Dequeue();

                if (Equals(expectedItem, actualItem))
                    continue;

                areEqual = false;
                break;
            }

            return areEqual;
        }
    }
}
