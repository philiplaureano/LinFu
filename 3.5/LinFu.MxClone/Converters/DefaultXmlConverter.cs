using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Extensions;
using System.Xml.Linq;
using Simple.IoC.Loaders;
namespace LinFu.MxClone
{
    [Implements(typeof(IXmlConverter), LifecycleType.OncePerRequest)]
    public class DefaultXmlConverter : IXmlConverter
    {
        #region IXmlConverter Members

        public IList<IMetaNode> Convert(XElement rootNode)
        {
            IList<IMetaNode> results = new List<IMetaNode>();
            rootNode.BuildNodeList(results);

            return results;
        }

        #endregion
    }
}
