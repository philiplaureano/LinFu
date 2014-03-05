using System;
using System.IO;
using LinFu.AOP.Cecil.Extensions;
using LinFu.Reflection.Emit;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace LinFu.AOP.Tasks
{
    /// <summary>
    /// Represents an MSBuild task for LinFu.AOP that allows users to inject an aspect framework into their applications
    /// at postbuild time.
    /// </summary>
    public class PostWeaveTask : Task
    {
        /// <summary>
        /// Gets or sets the value indicating the full path and filename of the target assembly.
        /// </summary>
        /// <value>The target assembly filename.</value>
        [Required]
        public string TargetFile { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the full path and filename of the output assembly.
        /// </summary>
        /// <value>The output assembly filename.</value>
        /// <remarks>This field is optional; if blank, the default value will be the same value as the <see cref="TargetFile"/> property.</remarks>
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not third party method calls should be intercepted in the target assembly.
        /// </summary>
        /// <value>A boolean value indicating whether or not third party method call interception should be enabled.</value>
        public bool InterceptAllMethodCalls { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not method bodies should be intercepted in the target assembly.
        /// </summary>
        /// <value>A boolean value indicating whether or not method body interception should be enabled.</value>
        public bool InterceptAllMethodBodies { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not new instances should be intercepted in the target assembly.
        /// </summary>
        /// <value>A boolean value indicating whether or not new instance interception should be enabled.</value>
        public bool InterceptAllNewInstances { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not field reads and writes should be intercepted in the target assembly.
        /// </summary>
        /// <value>A boolean value indicating whether or not field reads and writes should be enabled.</value>
        public bool InterceptAllFields { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not thrown exceptions should be intercepted in the target assembly.
        /// </summary>
        /// <value>A boolean value indicating whether or not exception interception should be enabled.</value>
        public bool InterceptAllExceptions { get; set; }

        /// <summary>
        /// Executes the postweaver.
        /// </summary>
        /// <returns>Returns <c>true</c> if the operation succeeded. Otherwise, it will return <c>false</c>.</returns>
        public override bool Execute()
        {
            // The output file name will be the same as the target
            // file by default 
            var outputFile = OutputFile;
            if (string.IsNullOrEmpty(outputFile))
                outputFile = TargetFile;

            var result = true;
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

                if (InterceptAllNewInstances)
                    assembly.InterceptAllNewInstances();

                if (InterceptAllMethodBodies)
                    assembly.InterceptAllMethodBodies();

                if (InterceptAllFields)
                    assembly.InterceptAllFields();

                if (InterceptAllExceptions)
                    assembly.InterceptAllExceptions();

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