using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;
using LinFu.AOP.Weavers.Cecil;
using System.IO;
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

            var assembly = AssemblyFactory.GetAssembly(targetFile);
            assembly.InjectAspectFramework(true);
            assembly.Save(targetFile);            
        }


        private static void ShowHelp()
        {
            Console.WriteLine("PostWeaver syntax: PostWeaver [filename]");
        }
    }
}
