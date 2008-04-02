using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinFu.MxClone.Interfaces
{
    public interface IMetaNode : ICloneable
    {
        int Depth { get; set; }
        int Sequence { get; set; }

        XElement TargetNode { get; set; }
        IMetaNode Parent { get; set; }
        IList<IMetaNode> Children { get; }
    }
}
