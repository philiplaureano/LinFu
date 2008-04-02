using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Indexes;
using LinFu.Reflection.Extensions;
using Simple.IoC.Extensions;
namespace LinFu.MxClone.Parsers
{
    public class CollectionNodeParser : IMetaNodeParser
    {
        public void Parse(IParserContext context)
        {
            var nodes = context.ParseList;
            var instantiationNodes = context.InstantiationNodes;
            var propertyIndexes = context.PropertyIndexes;
            var nodeTypeMap = context.NodeTypeMap;
            // Find the collection nodes.
            // Note: Collection nodes are typically found between two instantiation nodes

            // Select nodes which are not instantiation nodes
            var targetNodes = from n in nodes
                              where !instantiationNodes.Contains(n)
                              select n;

            var collectionNodes = (from n in targetNodes
                                   where instantiationNodes.Contains(n.Parent)
                                   && (from c in n.Children
                                       where instantiationNodes.Contains(c)
                                       select c).Count() > 0
                                   select n).ToList();


            // Each collection node must correspond to a property name on the given target type
            collectionNodes.ForEach(currentNode => MarkCollectionNode(context, propertyIndexes, nodeTypeMap, currentNode));
        }

        private static void MarkCollectionNode(IParserContext context, IDictionary<Type, IPropertyIndex> propertyIndexes, IDictionary<IMetaNode, Type> nodeTypeMap, IMetaNode currentNode)
        {
            var parent = currentNode.Parent;
            if (parent == null || !nodeTypeMap.ContainsKey(parent))
                return;

            // Get the type associated with the current instantiation node
            Type currentType = nodeTypeMap[parent];

            // Create a new property index and index the properties of this type, if necessary
            if (!propertyIndexes.ContainsKey(currentType))
                propertyIndexes[currentType] = new PropertyIndex(currentType);

            var currentIndex = propertyIndexes[currentType];
            var elementName = currentNode.TargetNode.Name;
            var propertyName = elementName.LocalName;

            // Collections should never have a prefix
            if (elementName.Namespace != context.DefaultNamespace)
                return;

            if (currentIndex.HasCollectionProperty(propertyName))
                context.CollectionNodes.Add(currentNode);
        }
    }
}
