using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinFu.MxClone.Interfaces
{
    public interface INodePreprocessor
    {
        IList<IMetaNode> Preprocess(IList<IMetaNode> nodes, IXmlConverter converter);
    }
}
