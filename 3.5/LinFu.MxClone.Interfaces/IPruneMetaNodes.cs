using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface IPruneMetaNodes
    {
        void Prune(IList<IMetaNode> nodes);
    }
}
