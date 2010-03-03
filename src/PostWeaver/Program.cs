using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Extensions;
using LinFu.Reflection.Emit;
using Mono.Cecil;

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

            var inputFile = args[0];            
            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            var targetFile = inputFile;
            var assembly = AssemblyFactory.GetAssembly(targetFile);

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(targetFile);
            var pdbFileName = string.Format("{0}.pdb", filenameWithoutExtension);
            var pdbExists = File.Exists(pdbFileName);

            var module = assembly.MainModule;

            if (pdbExists)
                module.LoadSymbols();

            assembly.InterceptAllNewInstances();
            assembly.InterceptAllMethodCalls();
            assembly.InterceptAllMethodBodies();
            
            assembly.InterceptAllFields();
            assembly.InterceptAllExceptions();

            // Update the PDB info if it exists
            if (pdbExists)
                module.SaveSymbols();

            assembly.Save(targetFile);

            Console.WriteLine("PostWeaving Assembly '{0}' -> '{0}'", targetFile);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("PostWeaver syntax: PostWeaver [filename]");
        }
    }
}
