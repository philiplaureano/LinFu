using System;
using System.IO;
using LinFu.AOP.CecilExtensions;
using LinFu.AOP.Weavers.Cecil;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using Simple.IoC;
using Simple.IoC.Loaders;

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
                
                var assemblyLocation = Path.GetDirectoryName(typeof(AspectWeaver).Assembly.Location);
                string taskLocation = Path.GetFullPath(assemblyLocation);

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
