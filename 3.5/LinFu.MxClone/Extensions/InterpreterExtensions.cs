using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;

using Simple.IoC.Extensions;
using Simple.IoC;
using Simple.IoC.Loaders;
namespace LinFu.MxClone.Extensions
{
    public static class InterpreterExtensions
    {
        public static void LoadFrom(this IMxInterpreter interpreter, IContainer container,
            string directory, string filespec)
        {
            if (interpreter == null)
                throw new ArgumentNullException("interpreter");

            // Populate each one of the collections using the matching item types
            // in each one of the assemblies in the target directory
            interpreter.ActionBuilders.CollectWith(container, directory, filespec);
            interpreter.NodeParsers.CollectWith(container, directory, filespec);
            interpreter.NodePruners.CollectWith(container, directory, filespec);
            interpreter.Preprocessors.CollectWith(container, directory, filespec);
        }
    }
}
