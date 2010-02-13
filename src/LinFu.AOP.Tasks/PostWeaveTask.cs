using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.AOP.Cecil;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace LinFu.AOP.Tasks
{
    public class PostWeaveTask : Task
    {
        [Required]
        public string TargetFile { get; set; }
        public string OutputFile { get; set; }

        public bool InterceptAllMethodCalls { get; set; }
        public bool InterceptAllMethodBodies { get; set; }
        public bool InterceptAllNewInstances { get; set; }
        public bool InterceptAllFields { get; set; }

        public override bool Execute()
        {
            // The output file name will be the same as the target
            // file by default 
            var outputFile = OutputFile;
            if (string.IsNullOrEmpty(outputFile))
                outputFile = TargetFile;

            bool result = true;
            try
            {
                Log.LogMessage("PostWeaving Assembly '{0}' -> '{1}'", TargetFile, outputFile);

                var assembly = AssemblyFactory.GetAssembly(TargetFile);

                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(TargetFile);
                var pdbFileName = string.Format("{0}.pdb", filenameWithoutExtension);
                var pdbExists = File.Exists(pdbFileName);

                var module = assembly.MainModule;

                if (pdbExists)
                    module.LoadSymbols();

                if (InterceptAllMethodCalls)
                    assembly.InterceptAllMethodCalls();

                if (InterceptAllMethodBodies)
                    assembly.InterceptAllMethodBodies();

                if (InterceptAllNewInstances)
                    assembly.InterceptAllNewInstances();

                if (InterceptAllFields)
                    assembly.InterceptAllFields();

                // Update the PDB info if it exists
                if (pdbExists)
                    module.SaveSymbols();

                assembly.Save(outputFile);
            }
            catch (Exception ex)
            {

                Log.LogErrorFromException(ex);
                result = false;
            }

            return result;
        }
    }
}
