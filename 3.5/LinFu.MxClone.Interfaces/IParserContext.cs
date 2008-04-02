using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinFu.MxClone.Interfaces
{
    public interface IParserContext
    {
        XNamespace DefaultNamespace { get; set; }
        IInstanceHolder InstanceHolder { get; set; }
        ICustomTypeDescriptor TypeDescriptor { get; set; }
        ITypeResolver TypeResolver { get; set; }
        HashSet<IMetaNode> AllNodes { get; }
        HashSet<IMetaNode> ParseList { get; }
        HashSet<IMetaNode> InstantiationNodes { get; }
        HashSet<IMetaNode> CollectionNodes { get; }
        IDictionary<IMetaNode, IInstance> InstanceMap { get; }
        IDictionary<IMetaNode, Type> NodeTypeMap { get; }
        IDictionary<Type, IPropertyIndex> PropertyIndexes { get; }
        IDictionary<Type, IEventIndex> EventIndexes { get; }
    }
}
