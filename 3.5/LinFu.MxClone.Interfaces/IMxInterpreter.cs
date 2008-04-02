using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
namespace LinFu.MxClone.Interfaces
{
    public interface IMxInterpreter
    {
        IList<IAction> Interpret(XElement root, IInstanceHolder holder);

        IList<IActionBuilder> ActionBuilders { get; }
        IList<Assembly> Assemblies { get; }
        IXmlConverter Converter { get; set; }
        IList<IMetaNodeParser> NodeParsers { get; }
        IList<IPruneMetaNodes> NodePruners { get; }
        IList<INodePreprocessor> Preprocessors { get; }
    }
}
