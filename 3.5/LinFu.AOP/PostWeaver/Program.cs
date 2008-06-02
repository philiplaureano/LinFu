using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;
using LinFu.AOP.Weavers.Cecil;
using System.IO;
using Simple.IoC;
using Simple.IoC.Loaders;
namespace PostWeaver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ShowHelp();
                return;
            }
            string targetFile = args[0];

            if (!File.Exists(targetFile))
                throw new FileNotFoundException(targetFile);

            Console.WriteLine("PostWeaving Assembly '{0}' -> '{1}'", targetFile, targetFile);

            // Search for any custom method filters that might
            // be located in the same directory as the postweaver
            var programLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            SimpleContainer container = new SimpleContainer();
            
            var loader = new Loader(container);
            loader.LoadDirectory(programLocation, "*.dll");

            IMethodFilter filter = null;
            filter = container.GetService<IMethodFilter>(false);

            var assembly = AssemblyFactory.GetAssembly(targetFile);
            assembly.InjectAspectFramework(filter, true);
            assembly.Save(targetFile); 
        }


        private static void ShowHelp()
        {
            Console.WriteLine("PostWeaver syntax: PostWeaver [filename]");
        }
    }
}
