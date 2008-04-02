using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Actions;
using LinFu.Reflection.Extensions;
using Simple.IoC.Extensions;

namespace LinFu.MxClone.ActionBuilders
{
    public class CreateNameRegistrations : IActionBuilder
    {
        public void Build(IParserContext context, IList<IAction> results)
        {
            if (context == null || results == null)
                return;

            var holder = context.InstanceHolder;
            var instanceMap = context.InstanceMap;
            var namedRegistrations = from key in instanceMap.Keys
                                     let currentNode = key.TargetNode
                                     where (from a in currentNode.Attributes()
                                            where a.Name.LocalName == "Name" &&
                                            a.Name.NamespaceName == "Definition"
                                            select a).Count() > 0
                                     select key;


            namedRegistrations.ForEach(nameRegistration => 
                AddNameRegistration(results, holder, 
                instanceMap, nameRegistration));            
        }

        private static void AddNameRegistration(IList<IAction> results, IInstanceHolder holder, IDictionary<IMetaNode, IInstance> instanceMap, IMetaNode nameRegistration)
        {
            var currentInstance = instanceMap[nameRegistration];
            var currentElement = nameRegistration.TargetNode;

            var nameAttribute = (from a in currentElement.Attributes()
                                 where a.Name.LocalName == "Name" &&
                                 a.Name.NamespaceName == "Definition"
                                 select a).First();

            var name = nameAttribute.Value;
            var registerAction = new RegisterInstanceAction(name, currentInstance, holder);
            results.Add(registerAction);
        }
    }
}
