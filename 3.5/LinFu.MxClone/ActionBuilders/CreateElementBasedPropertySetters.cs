using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Actions;
using LinFu.MxClone.Extensions;
using LinFu.Reflection.Extensions;

using Simple.IoC.Extensions;
namespace LinFu.MxClone.ActionBuilders
{
    [Collectable]
    public class CreateElementBasedPropertySetters : IActionBuilder
    {
        #region IActionBuilder Members

        public void Build(IParserContext context, IList<IAction> results)
        {
            if (context == null || results == null)
                return;

            var instantiationNodes = context.InstantiationNodes;

            // Limit the list to instantiation nodes
            // with known types
            var typedNodes = instantiationNodes.Where(n => context.NodeTypeMap.ContainsKey(n));
            var childNodes = from n in typedNodes
                             from c in n.Children
                             select c;

            // Find instantiation nodes with child elements that are
            // a set:PropertyName element
            var setterNodes = new List<IMetaNode>();
            foreach (var child in childNodes)
            {
                if (child.NamespaceName() == "Setters")
                    setterNodes.Add(child);

            }

            // Eliminate the setter nodes that don't correspond to
            // properties on their parent types
            var validSetters = from n in setterNodes
                               let propertyInfo = new
                               {
                                   ParentType = context.NodeTypeMap[n.Parent],
                                   PropertyName = n.LocalName()
                               }
                               where context.PropertyIndexes.ContainsKey(propertyInfo.ParentType)
                               && context.PropertyIndexes[propertyInfo.ParentType].HasProperty(propertyInfo.PropertyName)
                               select n;

            // Remove the setters with no child nodes
            validSetters = validSetters.Where(n => n.Children.Count > 0);


            // Remove the setters whose parent nodes do not correspond to any instances
            validSetters = (from v in validSetters
                           where context.InstanceMap.ContainsKey(v.Parent)
                           select v).ToList();

            validSetters.ForEach(setterNode => AssignPropertyValue(context, results, setterNode));            
        }

        private static void AssignPropertyValue(IParserContext context, IList<IAction> results, IMetaNode setterNode)
        {
            string propertyName = setterNode.LocalName();

            // Use only the first child node; all the other children
            // will be ignored by default
            var instanceKey = setterNode.Children.First();

            // Only add the child node instance if that instance exists
            if (!context.InstanceMap.ContainsKey(instanceKey))
                return;

            var instanceToAssign = context.InstanceMap[instanceKey];
            var parentInstance = context.InstanceMap[setterNode.Parent];

            var action = new AssignInstanceToPropertyValue(propertyName,
                instanceToAssign, parentInstance);

            results.Add(action);
        }

        #endregion
    }
}
