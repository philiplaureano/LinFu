using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Actions;
using Simple.IoC.Extensions;

namespace LinFu.MxClone.ActionBuilders
{
    public class CreateInstances : IActionBuilder
    {
        #region IActionBuilder Members

        public void Build(IParserContext context, IList<IAction> results)
        {
            var nodeTypeMap = context.NodeTypeMap;
            var instanceMap = context.InstanceMap;

            foreach (var node in context.InstantiationNodes)
            {
                // Get the type associated with this node
                if (!nodeTypeMap.ContainsKey(node))
                    continue;

                if (instanceMap.ContainsKey(node))
                    continue;

                // Link the current instance to the existing node
                Type targetType = nodeTypeMap[node];
                instanceMap[node] = new CreateInstance(targetType);
            }
        }

        #endregion
    }
}
