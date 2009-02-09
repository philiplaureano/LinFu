using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Loaders
{
    /// <summary>
    /// Represents a loader that modifies a given assembly prior to being loaded from disk.
    /// </summary>
    public class JITWeaver : AssemblyLoader
    {
        private readonly List<Action<AssemblyDefinition>> _assemblyWeavers = new List<Action<AssemblyDefinition>>();

        /// <summary>
        /// Modifies a given assembly prior to being loaded from disk.
        /// </summary>
        /// <param name="assemblyFile">The filename of the target assembly.</param>
        /// <returns>A valid assembly.</returns>
        public override Assembly Load(string assemblyFile)
        {
            var targetAssembly = AssemblyFactory.GetAssembly(assemblyFile);

            // Strongly-named assemblies cannot be modified
            if (targetAssembly.Name.HasPublicKey)
                return base.Load(assemblyFile);


            foreach (var action in AssemblyWeavers)
            {
                action(targetAssembly);

                // Verify the assembly at every step
                if (AssemblyVerifier == null)
                    continue;

                AssemblyVerifier.Verify(targetAssembly);
            }

            var memoryStream = new MemoryStream();
            AssemblyFactory.SaveAssembly(targetAssembly, memoryStream);

            // Load the modified assembly into memory
            var assembly = Assembly.Load(memoryStream.ToArray());

            return assembly;
        }

        /// <summary>
        /// Gets the value indicating the list of <see cref="Action{T}"/> delegates
        /// that will be used to modify the assemblies loaded into memory.
        /// </summary>
        public virtual IList<Action<AssemblyDefinition>> AssemblyWeavers
        {
            get { return _assemblyWeavers; }
        }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IVerifier"/>
        /// instance that will be used to ensure that the modified assemblies are valid.
        /// </summary>
        public virtual IVerifier AssemblyVerifier
        {
            get;
            set;
        }
    }
}