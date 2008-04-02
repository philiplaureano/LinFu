using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using System.Xml.Linq;

namespace LinFu.MxClone
{
    public class MetaNode : IMetaNode
    {
        private readonly List<IMetaNode> _children = new List<IMetaNode>();
        #region IMetaNode Members

        public int Depth
        {
            get;
            set;
        }

        public int Sequence
        {
            get;
            set;
        }

        public XElement TargetNode
        {
            get;
            set;
        }

        public IMetaNode Parent
        {
            get;
            set;
        }

        public IList<IMetaNode> Children
        {
            get
            {
                return _children;
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            MetaNode other = new MetaNode()
            {
                Depth = this.Depth,
                Parent = this.Parent,
                Sequence = this.Sequence,
                TargetNode = this.TargetNode
            };

            // Clone the children
            foreach (var child in _children)
            {
                IMetaNode clone = (IMetaNode)child.Clone();
                other.Children.Add(clone);
            }

            return other;
        }

        #endregion
    }
}
