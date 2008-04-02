using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Reflection;
using LinFu.MxClone.ActionBuilders;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Extensions;
using LinFu.MxClone.Parsers;
using LinFu.Reflection.Extensions;

using Simple.IoC;
using Simple.IoC.Loaders;


namespace LinFu.MxClone
{
    [Implements(typeof(IMxInterpreter), LifecycleType.Singleton)]
    public class MxInterpreter : IMxInterpreter, IInitialize
    {
        private IList<IMetaNodeParser> _parsers = new List<IMetaNodeParser>();
        private IList<IActionBuilder> _actionBuilders = new List<IActionBuilder>();
        private IList<IPruneMetaNodes> _pruners = new List<IPruneMetaNodes>();
        private IList<Assembly> _assemblies = new List<Assembly>();
        private IList<INodePreprocessor> _preprocessors = new List<INodePreprocessor>();
        private IContainer _container;
        private bool _loaded = false;
        public MxInterpreter()
        {
            // NOTE: These two parsers must be built in by default
            // in order for the parsing process to work properly
            _parsers.Add(new InstantiationNodeParser());
            _parsers.Add(new CollectionNodeParser());

            // The object instances must always be created and registered
            // first
            _actionBuilders.Add(new CreateInstances());
            _actionBuilders.Add(new CreateNameRegistrations());

            
        }
        public IList<INodePreprocessor> Preprocessors
        {
            get { return _preprocessors; }
        }
        public IList<Assembly> Assemblies
        {
            get { return _assemblies; }
        }
        public IList<IPruneMetaNodes> NodePruners
        {
            get
            {
                return _pruners;
            }
        }
        public IXmlConverter Converter { get; set; }
        public IList<IMetaNodeParser> NodeParsers
        {
            get { return _parsers; }
        }
        public IList<IActionBuilder> ActionBuilders
        {
            get { return _actionBuilders; }
        }

        public IList<IAction> Interpret(XElement element, IInstanceHolder holder)
        {
            if (Converter == null)
                return new List<IAction>();

            XElement root = element;
            var nodes = Converter.Convert(root);

            // Perform any necessary XML transformations
            if (Preprocessors != null)
                Preprocessors.ForEach(p => p.Preprocess(nodes, Converter));
            IParserContext context = _container.GetService<IParserContext>();
            if (context == null)
                return new List<IAction>();

            InitializeContext(context, root, holder);

            // Copy the converted nodes to the context
            nodes.ForEach(n => context.AllNodes.Add(n));

            // Determine which nodes need to be excluded from the parse list
            if (NodePruners != null)
                NodePruners.ForEach(n => n.Prune(nodes));
          
            // Copy the pruned nodes to the parse list
            nodes.ForEach(n => context.ParseList.Add(n));

            // Run the converted nodes through each one of the parsers,
            // have the parsers interpret each of the nodes, and
            // save the interpreted results back to the context
            Action<IMetaNodeParser> parseNode = (parser) => parser.Parse(context);
            parseNode.Fold(NodeParsers);

            var actions = new List<IAction>();

            // Convert the context into a set of actions
            Action<IActionBuilder> buildAction = (builder) => builder.Build(context, actions);
            buildAction.Fold(ActionBuilders);

            return actions;
        }

        private void InitializeContext(IParserContext context, XElement root, IInstanceHolder holder)
        {
            var defaultNamespace = root.Name.Namespace;
            context.DefaultNamespace = defaultNamespace;

            // Assign the default namespace
            context.TypeDescriptor = _container.GetService<ICustomTypeDescriptor>();

            // Use the given instance holder, if possible
            context.InstanceHolder = holder ?? _container.GetService<IInstanceHolder>();

            var compositeResolver = new CompositeTypeResolver();
            var resolver = _container.GetService<ITypeResolver>();

            // Add the original resolver by default
            if (resolver != null)
                compositeResolver.Resolvers.Add(resolver);

            // Add the additional assemblies to the
            // list of assemblies the interpreter will use
            // for type resolution
            _assemblies.ForEach(a => compositeResolver.AddAssembly(a));

            context.TypeResolver = compositeResolver;
        }
       
        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            if (container == null || _loaded)
                return;

            _container = container;
            Converter = _container.GetService<IXmlConverter>();

            // Load all the additional plugins for this interpreter
            string directory = Path.GetDirectoryName(typeof(IMxInterpreter).Assembly.Location);
            this.LoadFrom(container, directory, "*.dll");

            _loaded = true;
        }

        #endregion
    }
}
