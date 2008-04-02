using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection.Extensions;

namespace LinFu.MxClone.Parsers
{
    public class InstantiationNodeParser : IMetaNodeParser
    {
        #region IInstantiationNodeParser Members

        public void Parse(IParserContext context)
        {
            var nodes = context.ParseList;
            var resolver = context.TypeResolver;
            var nodeTypeMap = context.NodeTypeMap;

            nodes.ForEach(node => MarkInstantiationNode(context, resolver, nodeTypeMap, node));           
        }

        private static void MarkInstantiationNode(IParserContext context, ITypeResolver resolver, IDictionary<IMetaNode, Type> nodeTypeMap, IMetaNode node)
        {
            var nodeName = node.TargetNode.Name;
            string typename = nodeName.LocalName;
            string currentNamespace = nodeName.NamespaceName;

            Type currentType = resolver.Resolve(typename, currentNamespace);
            if (currentType != null)
            {

                // Keep track of which type is associated with the current node
                nodeTypeMap[node] = currentType;
                context.InstantiationNodes.Add(node);
            }
        }

        #endregion
    }
}
