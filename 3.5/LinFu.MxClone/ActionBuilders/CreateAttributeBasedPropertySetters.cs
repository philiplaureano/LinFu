using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Indexes;
using LinFu.MxClone.Actions;
using Simple.IoC.Extensions;

namespace LinFu.MxClone.ActionBuilders
{
    [Collectable]
    public class CreateAttributeBasedPropertySetters : IActionBuilder
    {
        public void Build(IParserContext context, IList<IAction> results)
        {
            if (context == null || results == null)
                return;

            var nodeTypeMap = context.NodeTypeMap;
            var instanceMap = context.InstanceMap;
            var instantiationNodes = context.InstantiationNodes;
            foreach (var node in instantiationNodes)
            {
                // Get the type associated with this node
                if (!nodeTypeMap.ContainsKey(node))
                    continue;

                Type targetType = nodeTypeMap[node];
                AddPropertyActions(node, context, targetType, results);
            }
        }

        private void AddPropertyActions(IMetaNode currentNode, IParserContext context, Type targetType, IList<IAction> results)
        {
            var propertyIndexes = context.PropertyIndexes;
            // Index the properties on this current type
            if (!propertyIndexes.ContainsKey(targetType))
                propertyIndexes[targetType] = new PropertyIndex(targetType);

            var currentElement = currentNode.TargetNode;

            // Search the current element for any property attributes
            var propertyAttributes = from a in currentElement.Attributes()
                                     where string.IsNullOrEmpty(a.Name.Namespace.NamespaceName) &&
                                     propertyIndexes[targetType].HasProperty(a.Name.LocalName)
                                     select new { PropertyName = a.Name.LocalName, Value = a.Value };

            var instanceMap = context.InstanceMap;
            var nodeTypeMap = context.NodeTypeMap;
            var holder = context.InstanceHolder;

            // Determine if the parent type can be 
            // known 'a priori'
            var parentType = nodeTypeMap.ContainsKey(currentNode) ? 
                nodeTypeMap[currentNode] : typeof(object);

            foreach (var attribute in propertyAttributes)
            {
                var propertyName = attribute.PropertyName;
                var value = attribute.Value;
                SetPropertyValue(currentNode, context, results, holder, propertyName, value);
                
            }
        }

        private static void SetPropertyValue(IMetaNode currentNode, IParserContext context, 
            IList<IAction> results, IInstanceHolder holder, string propertyName, string value)
        {
            if (holder == null)
                return;

            IAction assignAction = null;
            var instanceMap = context.InstanceMap;

            // Determine if we're referencing a named instance
            bool isStandardAssignment = !value.StartsWith("{") || !value.EndsWith("}") || value.Length <= 2;

            if (isStandardAssignment)
            {
                var target = instanceMap[currentNode];
                assignAction = new AssignPropertyValue(propertyName, value, target, context);
                results.Add(assignAction);
                return;
            }

            // Extract the instance name
            int nameLength = value.Length - 2;
            string instanceName = value.Substring(1, nameLength);


            assignAction = new AssignNamedInstanceToPropertyValue(propertyName, instanceName,
                instanceMap[currentNode], holder);

            results.Add(assignAction);
        }
    }
}
