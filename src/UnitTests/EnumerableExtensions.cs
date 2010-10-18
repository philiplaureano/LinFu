using System.Collections.Generic;

namespace LinFu.UnitTests
{
    public static class EnumerableExtensions
    {
        public static bool AreEqualTo<T>(this IEnumerable<T> expectedArray, IEnumerable<T> actualArray)
        {
            bool areEqual = true;

            var expectedItems = new Queue<T>(expectedArray);
            var actualItems = new Queue<T>(actualArray);

            if (expectedItems.Count != actualItems.Count)
                return false;

            while (expectedItems.Count > 0)
            {
                T expectedItem = expectedItems.Dequeue();
                T actualItem = actualItems.Dequeue();

                if (Equals(expectedItem, actualItem))
                    continue;

                areEqual = false;
                break;
            }

            return areEqual;
        }
    }
}