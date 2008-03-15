using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Weavers.Cecil;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.AOP.Tasks
{
    public class PostWeaveTask : Task
    {
        [Required]
        public string TargetFile { get; set; }
        public string OutputFile { get; set; }
        public bool InjectConstructors
        {
            get;
            set;
        }
        public override bool Execute()
        {
            // The output file name will be the same as the target
            // file by default 
            string outputFile = OutputFile;
            if (string.IsNullOrEmpty(outputFile))
                outputFile = TargetFile;

            bool result = true;
            try
            {
                Log.LogMessage("PostWeaving Assembly '{0}' -> '{1}'", TargetFile, outputFile);

                string taskLocation = typeof(AspectWeaver).Assembly.Location;

                // Search for any custom method filters that might
                // be located in the same directory as the aspect weaver
                SimpleContainer container = new SimpleContainer();
                var loader = new Loader(container);
                loader.LoadDirectory(taskLocation, "*.dll");

                IMethodFilter filter = null;
                filter = container.GetService<IMethodFilter>(false);
                
                // Modify the assembly and apply the filter, if necessary
                var assembly = AssemblyFactory.GetAssembly(TargetFile);
                assembly.InjectAspectFramework(filter, InjectConstructors);

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
