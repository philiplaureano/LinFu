using System.Linq;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    ///     A helper class that extends Cecil to support LinFu's weaver model.
    /// </summary>
    public static class CecilVisitorExtensions
    {
        /// <summary>
        /// Applies the Type transformation to the target type.
        /// </summary>
        /// <param name="host">The target type</param>
        /// <param name="typeWeaver">The type weaver that will make the current set of modifications.</param>
        public static void Accept(this TypeDefinition host, ITypeWeaver typeWeaver)
        {
            typeWeaver.Weave(host);
        }
        
        /// <summary>
        /// Applies the Type transformation to the target type.
        /// </summary>
        /// <param name="host">The target type</param>
        /// <param name="typeWeaver">The type weaver that will make the current set of modifications.</param>
        public static void Accept(this AssemblyDefinition host, ITypeWeaver typeWeaver)
        {
            var module = host.MainModule;
            var types = module.Types.Where(typeWeaver.ShouldWeave).ToArray();
            foreach (var type in types)
            {
                typeWeaver.Weave(type);
            }
        }
        
        /// <summary>
        ///     Allows a <see cref="ITypeWeaver" /> instance to traverse any <see cref="IReflectionVisitable" />
        ///     instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="typeWeaver">The type weaver.</param>
        public static void Accept(this IReflectionVisitable visitable, ITypeWeaver typeWeaver)
        {
            var visitor = new TypeWeaverVisitor(typeWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        ///     Allows a <see cref="ITypeWeaver" /> instance to traverse any <see cref="IReflectionStructureVisitable" />
        ///     instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="typeWeaver">The type weaver.</param>
        public static void Accept(this IReflectionStructureVisitable visitable, ITypeWeaver typeWeaver)
        {
            var visitor = new TypeWeaverVisitor(typeWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        ///     Allows a <see cref="IMethodWeaver" /> instance to traverse any <see cref="IReflectionVisitable" />
        ///     instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="methodWeaver">The method weaver.</param>
        public static void Accept(this IReflectionStructureVisitable visitable, IMethodWeaver methodWeaver)
        {
            var visitor = new MethodWeaverVisitor(methodWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        ///     Allows a <see cref="IMethodWeaver" /> instance to traverse any <see cref="IReflectionVisitable" />
        ///     instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="methodWeaver">The method weaver.</param>
        public static void Accept(this IReflectionVisitable visitable, IMethodWeaver methodWeaver)
        {
            var visitor = new MethodWeaverVisitor(methodWeaver);
            visitable.Accept(visitor);
        }
    }
}