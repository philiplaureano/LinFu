using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodAttributes=Mono.Cecil.MethodAttributes;
using MethodImplAttributes=Mono.Cecil.MethodImplAttributes;
using TypeAttributes=Mono.Cecil.TypeAttributes;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that provides helper extension methods
    /// for the <see cref="ModuleDefinition"/> class.
    /// </summary>
    public static class ModuleDefinitionExtensions
    {
        /// <summary>
        /// Defines a new class and adds it to the <paramref name="mainModule"/> module.
        /// </summary>
        /// <param name="mainModule">The module which will hold the new created type.</param>
        /// <param name="typeName">The name of the class to create.</param>
        /// <param name="namespaceName">The namespace that will contain the new class.</param>
        /// <param name="attributes">The <see cref="Mono.Cecil.TypeAttributes"/> for the given type.</param>
        /// <param name="baseType">The base class of the new type.</param>
        /// <returns>A <see cref="TypeDefinition"/> representing the new class being created.</returns>
        public static TypeDefinition DefineClass(this ModuleDefinition mainModule,
            string typeName, string namespaceName, TypeAttributes attributes,
            TypeReference baseType)
        {
            var resultType = new TypeDefinition(typeName, namespaceName,
                                               attributes, baseType);

            mainModule.Types.Add(resultType);
            return resultType;
        }
        
        /// <summary>
        /// Imports a constructor with the given <paramref name="constructorParameters"/>
        /// into the target <paramref name="module"/>.
        /// </summary>
        /// <typeparam name="T">The type that holds the target constructor</typeparam>
        /// <param name="module">The <see cref="ModuleDefinition"/> that will import the target constructor.</param>
        /// <param name="constructorParameters">The list of <see cref="System.Type"/> objects that describe the signature of the constructor.</param>
        /// <returns>A <see cref="MethodReference"/> that represents the constructor itself.</returns>
        public static MethodReference ImportConstructor<T>(this ModuleDefinition module, params Type[] constructorParameters)
        {
            return module.Import(typeof(T).GetConstructor(constructorParameters));
        }

        /// <summary>
        /// Imports a method with a particular <paramref name="methodName"/> from the <paramref name="declaringType"/>
        /// into the <paramref name="module">target module</paramref>.
        /// </summary>
        /// <param name="module">The <see cref="ModuleDefinition"/> instance that will import the actual method.</param>
        /// <param name="methodName">The name of the method being imported.</param>
        /// <param name="declaringType">The <see cref="System.Type"/> instance that holds the target method.</param>
        /// <returns>A <see cref="MethodReference"/> that represents the method being imported.</returns>
        public static MethodReference ImportMethod(this ModuleDefinition module, string methodName, Type declaringType)
        {
            return module.Import(declaringType.GetMethod(methodName));
        }

        /// <summary>
        /// Imports a method with a particular <paramref name="methodName"/> and <see cref="BindingFlags"/> from the <paramref name="declaringType"/>
        /// into the <paramref name="module">target module</paramref>.
        /// </summary>
        /// <param name="module">The <see cref="ModuleDefinition"/> instance that will import the actual method.</param>
        /// <param name="methodName">The name of the method being imported.</param>
        /// <param name="declaringType">The <see cref="System.Type"/> instance that holds the target method.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> that describes the visibility and behavior of the target method.</param>
        /// <returns>A <see cref="MethodReference"/> that represents the method being imported.</returns>
        public static MethodReference ImportMethod(this ModuleDefinition module, string methodName, Type declaringType, BindingFlags flags)
        {
            return module.Import(declaringType.GetMethod(methodName, flags));
        }

        /// <summary>
        /// Imports a method with a particular <paramref name="methodName"/> and <see cref="BindingFlags"/> from the <paramref name="declaringType"/>
        /// into the <paramref name="module">target module</paramref>.
        /// </summary>
        /// <typeparam name="T">The target type that holds the target method.</typeparam>
        /// <param name="module">The <see cref="ModuleDefinition"/> instance that will import the actual method.</param>
        /// <param name="methodName">The name of the method being imported.</param>
        /// <returns>A <see cref="MethodReference"/> that represents the method being imported.</returns>
        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName)
        {
            return module.Import(typeof(T).GetMethod(methodName));
        }

        /// <summary>
        /// Imports a method with a particular <paramref name="methodName"/>, <paramref name="parameterTypes"/>, and <see cref="BindingFlags"/> from the <paramref name="declaringType"/>
        /// into the <paramref name="module">target module</paramref>.
        /// </summary>
        /// <typeparam name="T">The target type that holds the target method.</typeparam>
        /// <param name="parameterTypes">The list of <see cref="Type"/> objects that describe the method signature.</param>
        /// <param name="module">The <see cref="ModuleDefinition"/> instance that will import the actual method.</param>
        /// <param name="methodName">The name of the method being imported.</param>
        /// <returns>A <see cref="MethodReference"/> that represents the method being imported.</returns>
        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName, params Type[] parameterTypes)
        {
            return module.Import(typeof(T).GetMethod(methodName, parameterTypes));
        }

        /// <summary>
        /// Imports a method with a particular <paramref name="methodName"/> and <see cref="BindingFlags"/> from the <paramref name="declaringType"/>
        /// into the <paramref name="module">target module</paramref>.
        /// </summary>
        /// <typeparam name="T">The target type that holds the target method itself.</typeparam>
        /// <param name="module">The <see cref="ModuleDefinition"/> instance that will import the actual method.</param>
        /// <param name="methodName">The name of the method being imported.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> that describes the visibility and behavior of the target method.</param>
        /// <returns>A <see cref="MethodReference"/> that represents the method being imported.</returns>
        public static MethodReference ImportMethod<T>(this ModuleDefinition module,
            string methodName, BindingFlags flags)
        {
            return module.Import(typeof(T).GetMethod(methodName, flags));
        }

        /// <summary>
        /// Imports a target of type <typeparamref name="T"/> into the
        /// <paramref name="module"/> instance.
        /// </summary>
        /// <typeparam name="T">The type that will be imported into the <see cref="ModuleDefinition"/> instance itself.</typeparam>
        /// <param name="module">The module that will store the imported type.</param>
        /// <returns>A <see cref="TypeReference"/> instance that represents the imported type.</returns>
        public static TypeReference ImportType<T>(this ModuleDefinition module)
        {
            return module.Import(typeof(T));
        }

        /// <summary>
        /// Imports a <paramref name="targetType">target type</paramref> into the
        /// <paramref name="module"/> instance.
        /// </summary>
        /// <param name="targetType">The type that will be imported into the <see cref="ModuleDefinition"/> instance itself.</param>
        /// <param name="module">The module that will store the imported type.</param>
        /// <returns>A <see cref="TypeDefinition"/> instance that represents the imported type.</returns>
        public static TypeReference ImportType(this ModuleDefinition module, Type targetType)
        {
            return module.Import(targetType);
        }

        /// <summary>
        /// Returns a <see cref="TypeDefinition"/> that matches the given <paramref name="typeName"/>.
        /// </summary>
        /// <param name="module">The target module to search.</param>
        /// <param name="typeName">The name of the target type.</param>
        /// <returns>A type that matches the given type name. If the type cannot be found, then this method will return <c>null</c>.</returns>
        public static TypeDefinition GetType(this ModuleDefinition module, string typeName)
        {
            TypeDefinition result = (from TypeDefinition t in module.Types
                          where t.Name == typeName
                          select t).FirstOrDefault();

            return result;
        }
    }
}
