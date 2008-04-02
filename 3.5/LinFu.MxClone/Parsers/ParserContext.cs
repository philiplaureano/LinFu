using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using Simple.IoC.Loaders;
using Simple.IoC;
using System.Xml.Linq;

namespace LinFu.MxClone
{
    [Implements(typeof(IParserContext), LifecycleType.OncePerRequest)]
    public class ParserContext : IParserContext, IInitialize
    {
        private HashSet<IMetaNode> _allNodes = new HashSet<IMetaNode>();
        private HashSet<IMetaNode> _parsedNodes = new HashSet<IMetaNode>();
        private HashSet<IMetaNode> _instantiationNodes = new HashSet<IMetaNode>();
        private HashSet<IMetaNode> _collectionNodes = new HashSet<IMetaNode>();
        private IInstanceHolder _holder = new DefaultInstanceHolder();
        private IDictionary<IMetaNode, Type> _nodeTypeMap = new Dictionary<IMetaNode, Type>();
        private IDictionary<IMetaNode, IInstance> _instanceMap = new Dictionary<IMetaNode, IInstance>();
        private IDictionary<Type, IPropertyIndex> _propertyIndexes = new Dictionary<Type, IPropertyIndex>();
        private IDictionary<Type, IEventIndex> _eventIndexes = new Dictionary<Type, IEventIndex>();
        #region IParserContext Members

        public XNamespace DefaultNamespace { get; set; }
        public ITypeResolver TypeResolver
        {
            get;
            set;
        }
        public ICustomTypeDescriptor TypeDescriptor
        {
            get;
            set;
        }
        public HashSet<IMetaNode> AllNodes
        {
            get { return _allNodes; }
        }
        public HashSet<IMetaNode> ParseList
        {
            get { return _parsedNodes; }
        }
        public IDictionary<IMetaNode, Type> NodeTypeMap
        {
            get { return _nodeTypeMap; }
        }
        public IDictionary<Type, IPropertyIndex> PropertyIndexes
        {
            get { return _propertyIndexes; }
        }
        public HashSet<IMetaNode> InstantiationNodes
        {
            get { return _instantiationNodes; }
        }

        public HashSet<IMetaNode> CollectionNodes
        {
            get { return _collectionNodes; }
        }

        public IDictionary<IMetaNode, IInstance> InstanceMap
        {
            get { return _instanceMap; }
        }

        public IInstanceHolder InstanceHolder
        {
            get
            {
                return _holder;
            }
            set
            {
                _holder = value;
            }
        }

        public IDictionary<Type, IEventIndex> EventIndexes
        {
            get { return _eventIndexes; }
        }
        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInitialize Members

        void IInitialize.Initialize(IContainer container)
        {
            TypeResolver = container.GetService<ITypeResolver>();
        }

        #endregion
    }
}
