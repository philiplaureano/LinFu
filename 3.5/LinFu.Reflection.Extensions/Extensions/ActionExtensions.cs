using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public static class ActionExtensions
    {
        public static void Fold<T>(this Action<T> action, params T[] subjects)
        {
            Fold(action, (IEnumerable<T>)subjects);
        }
        public static void Fold<T>(this Action<T> action, IEnumerable<T> subjects)
        {
            if (action == null || subjects == null)
                return;

            subjects.ForEach(action);
        }
    }
}
