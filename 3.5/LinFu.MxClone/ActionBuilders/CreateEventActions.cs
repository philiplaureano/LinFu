using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Indexes;
using LinFu.MxClone.Actions;
using LinFu.Reflection.Extensions;
using Simple.IoC.Extensions;


namespace LinFu.MxClone.ActionBuilders
{
    [Collectable]
    public class CreateEventActions : IActionBuilder
    {
        #region IMetaNodeParser Members

        public void Build(IParserContext context, IList<IAction> results)
        {
            var instantiationNodes = context.InstantiationNodes;
            foreach (var node in instantiationNodes)
            {
                Parse(node, context, results);
            }
        }

        private static void Parse(IMetaNode currentNode, IParserContext context, IList<IAction> results)
        {
            if (!context.NodeTypeMap.ContainsKey(currentNode))
                return;

            // Create the event index if necessary
            Type currentType = context.NodeTypeMap[currentNode];
            if (!context.EventIndexes.ContainsKey(currentType))
                context.EventIndexes[currentType] = new EventIndex(currentType);

            var eventIndex = context.EventIndexes[currentType];

            // Find any attributes correspond with any events declared
            // on the target type
            var currentElement = currentNode.TargetNode;
            var eventAttributes = from a in currentElement.Attributes()
                                  where eventIndex.HasEvent(a.Name.LocalName) &&
                                  string.IsNullOrEmpty(a.Name.NamespaceName) &&
                                  a.Name.NamespaceName != "Definition"
                                  select a;


            IInstance source = context.InstanceMap[currentNode];
            if (source == null)
                return;

            eventAttributes.ForEach(attribute => 
                AddEventBinding(context, results, source, attribute));            
        }

        private static void AddEventBinding(IParserContext context, IList<IAction> results, 
            IInstance source, XAttribute attribute)
        {
            string eventName = attribute.Name.LocalName;
            string handlerValue = attribute.Value;

            var parsedItems = handlerValue.Split('.');
            int itemCount = parsedItems.Count();

            // There must be an instance name and a method 
            // to bind to the target event
            if (itemCount < 2)
                return;

            var instanceName = parsedItems[0];
            var methodName = parsedItems[1];

            if (string.IsNullOrEmpty(instanceName) || string.IsNullOrEmpty(methodName))
                return;

            var bindToEvent = new BindToEvent(eventName, instanceName,
                    methodName, source, context.InstanceHolder);

            results.Add(bindToEvent);
        }

        #endregion
    }
}
