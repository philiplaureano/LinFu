using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LinFu.MxClone.Interfaces;
using LinFu.Reflection.Extensions;
using Simple.IoC.Loaders;
using Simple.IoC;
using System.Xml.Linq;
using System.IO;
using System.Xml;
namespace LinFu.MxClone
{
    [Implements(typeof(IMxEngine), LifecycleType.OncePerRequest)]
    public class MxEngine : IMxEngine, IInitialize
    {
        #region IMxEngine Members

        public IMxInterpreter Interpreter
        {
            get;
            set;
        }

        public void Execute(string mxFilename, IInstanceHolder holder)
        {
            if (Interpreter == null)
                return;
            
            XElement root = ReadXmlFile(mxFilename);
            var actions = Interpreter.Interpret(root, holder);

            // Execute the graph
            actions.ForEach(a => a.Execute());
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            Interpreter = container.GetService<IMxInterpreter>();
        }

        #endregion

        private static XElement ReadXmlFile(string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Open);
            var reader = new XmlTextReader(file);
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;
            return root;
        }

    }
}
