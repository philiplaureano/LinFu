using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// The exception thrown when a recursive dependency is detected
    /// inside a <see cref="IServiceContainer"/> instance.
    /// </summary>
    public class RecursiveDependencyException : Exception
    {
        private LinkedList<Type> _typeChain;

        /// <summary>
        /// Initializes the <see cref="RecursiveDependencyException"/>
        /// class with the <paramref name="typeChain">chain</paramref>
        /// of depedencies that caused the exception.
        /// </summary>
        /// <param name="typeChain">The sequence of types that caused the dependency exception.</param>
        public RecursiveDependencyException(LinkedList<Type> typeChain)
        {
            _typeChain = typeChain;
        }

        /// <summary>
        /// Gets the value indicating the chain of types that caused the exception.
        /// </summary>
        public LinkedList<Type> TypeChain
        {
            get
            {
                // Prevent users from modifying the actual list
                return new LinkedList<Type>(_typeChain);
            }
        }
        
        /// <summary>
        /// Gets the value indicating the error message from the <see cref="RecursiveDependencyException"/>.
        /// </summary>
        public override string Message
        {
            get
            {
                var messageFormat = "Recursive Dependency Detected: {0}";
                var builder = new StringBuilder();

                var currentNode = _typeChain.First;
                while(currentNode != null)
                {
                    builder.AppendFormat("{0}", currentNode.Value.AssemblyQualifiedName);

                    if (currentNode.Next != null)
                    {
                        builder.Append("--->");
                    }
                    currentNode = currentNode.Next;
                }

                return string.Format(messageFormat, builder);
            }
        }
    }
}
