using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    public class CollectionLoader<T> : IActionLoader<ICollection<T>, Type>
        where T : class
    {
        public IEnumerable<Action<ICollection<T>>> Load(Type input)
        {
            var actionList = new List<Action<ICollection<T>>>();

            var component = (T)Activator.CreateInstance(input);
            actionList.Add(items => items.Add(component));

            return actionList;
        }

        public bool CanLoad(Type inputType)
        {
            if (!typeof(T).IsAssignableFrom(inputType))
                return false;

            if (!inputType.IsClass)
                return false;

            if (inputType.IsAbstract)
                return false;

            return true;
        }
    }
}
