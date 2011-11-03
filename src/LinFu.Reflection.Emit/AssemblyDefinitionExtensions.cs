﻿using System.IO;
using System.Reflection;
using Mono.Cecil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that adds extension methods to the <see cref="AssemblyDefinition"/>
    /// class.
    /// </summary>
    public static class AssemblyDefinitionExtensions
    {
        /// <summary>
        /// Converts an <see cref="AssemblyDefinition"/>
        /// into a running <see cref="Assembly"/>.
        /// </summary>
        /// <param name="definition">The <see cref="AssemblyDefinition"/> to convert.</param>
        /// <returns>An <see cref="Assembly"/> that represents the <see cref="AssemblyDefinition"/> instance.
        /// </returns>
        public static Assembly ToAssembly(this AssemblyDefinition definition)
        {
            Assembly result = null;
            using (var stream = new MemoryStream())
            {
                // Persist the assembly to the stream
				definition.Write(stream);
                byte[] buffer = stream.GetBuffer();
                result = Assembly.Load(buffer);
            }
        	definition.Write(@"D:\tools\LinFu\source\LinFu\build\Debug\UnitTests\new.dll");

            return result;
        }

        /// <summary>
        /// Saves the assembly to disk.
        /// </summary>
        /// <param name="definition">The target assembly definition.</param>
        /// <param name="filename">The output file name.</param>
        public static void Save(this AssemblyDefinition definition, string filename)
        {
        	definition.Write(filename);
        }

        /// <summary>
        /// Saves the assembly to disk.
        /// </summary>
        /// <param name="definition">The target assembly definition.</param>
        /// <param name="outputStream">The destination file stream.</param>
        public static void Save(this AssemblyDefinition definition, Stream outputStream)
        {
			definition.Write(outputStream);
        }
    }
}