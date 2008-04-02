using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace Simple.IoC.Extensions
{
    public class CollectableLoader<T> : BaseLoader
        where T : class
    {
        public CollectableLoader(IList<T> results, IContainer container)
        {
            LoadStrategy = new CollectableLoadStrategy<T>(results, container);
        }
    }
}
