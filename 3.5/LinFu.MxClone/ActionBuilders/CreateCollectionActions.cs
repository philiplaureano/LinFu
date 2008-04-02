using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Actions;
using Simple.IoC.Extensions;

namespace LinFu.MxClone.ActionBuilders
{
    [Collectable]
    public class CreateCollectionActions : IActionBuilder
    {
        public void Build(IParserContext context, IList<IAction> results)
        {
            if (context == null || results == null)
                return;

            // Populate all of the collections with the newly-created object instances
            var collectionNodeList = context.CollectionNodes;
            foreach (var collectionNode in collectionNodeList)
            {
                CreateActions(collectionNode, context, results);
            }
        }

        private void CreateActions(IMetaNode currentCollectionNode, IParserContext context, IList<IAction> results)
        {
            var nodeName = currentCollectionNode.TargetNode.Name;
            var collectionPropertyName = nodeName.LocalName;

            var instantiationNodes = context.InstantiationNodes;
            var instanceMap = context.InstanceMap;
            // Link the parents and the children together
            var parentNode = currentCollectionNode.Parent;
            var childNodes = currentCollectionNode.Children.Where(c => instantiationNodes.Contains(c));

            if (!instanceMap.ContainsKey(parentNode))
                return;

            var parentInstance = instanceMap[parentNode];
            foreach (var child in childNodes)
            {
                if (!instanceMap.ContainsKey(child))
                    continue;

                var childInstance = instanceMap[child];
                var addCollectionItem = new AddCollectionItem(collectionPropertyName, parentInstance, childInstance);
                results.Add(addCollectionItem);
            }

        }
    }
}
