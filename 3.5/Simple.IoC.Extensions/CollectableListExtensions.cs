using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC.Extensions
{
    public static class CollectableListExtensions
    {
        public static void CollectWith<T>(this IList<T> list, IContainer container, 
            string directory, string filespec)
           where T : class
        {
            // Load all types that are compatible with type T and
            // have the [Collectable] attribute defined
            // and intialize it with the given container
            var loader = new CollectableLoader<T>(list, container);
            loader.LoadDirectory(directory, filespec);
        }
    }
}
