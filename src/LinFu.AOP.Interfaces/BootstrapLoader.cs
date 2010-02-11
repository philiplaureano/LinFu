using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.AOP.Interfaces
{
    internal class BootstrapLoader : IActionLoader<IList<IBootStrappedComponent>, Type>
    {
        public IEnumerable<Action<IList<IBootStrappedComponent>>> Load(Type input)
        {
            var actionList = new List<Action<IList<IBootStrappedComponent>>>();

            var component = (IBootStrappedComponent)Activator.CreateInstance(input);
            actionList.Add(items => items.Add(component));

            return actionList;
        }

        public bool CanLoad(Type inputType)
        {
            var interfaces = inputType.GetInterfaces();
            if (!interfaces.Contains(typeof(IBootStrappedComponent)))
                return false;

            if (!inputType.IsClass)
                return false;

            if (inputType.IsAbstract)
                return false;            

            return true;
        }
    }
}
