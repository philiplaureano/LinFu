using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using LinFu.AOP.Cecil.Interfaces;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// An extension class that adds support for intercepting the 'new' operator with LinFu.AOP.
    /// </summary>
    public static class NewOperatorInterceptionExtensions
    {

        /// <summary>
        /// Modifies a <paramref name="target"/> to support intercepting all calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The assembly to be modified.</param>
        public static void InterceptAllNewInstances(this IReflectionStructureVisitable target)
        {
            Func<TypeReference, bool> typeFilter = type =>
            {
                var actualType = type.Resolve();
                return actualType.IsClass && !actualType.IsInterface;
            };

            target.InterceptNewInstances(typeFilter);
        }

        /// <summary>
        /// Modifies a <paramref name="target"/> to support intercepting all calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The assembly to be modified.</param>
        public static void InterceptAllNewInstances(this IReflectionVisitable target)
        {
            Func<TypeReference, bool> typeFilter = type =>
                                                       {
                                                           var actualType = type.Resolve();
                                                           return actualType.IsClass && !actualType.IsInterface;
                                                       };

            target.InterceptNewInstances(typeFilter);
        }
        

        /// <summary>
        /// Modifies a <paramref name="target"/> to support intercepting calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The assembly to be modified.</param>
        /// <param name="typeFilter">The functor that determines which type instantiations should be intercepted.</param>
        /// <param name="methodFilter">The filter that determines which host methods will be modified</param>
        /// <remarks>
        /// The type filter determines the concrete types that should be intercepted at runtime.
        /// For example, the following functor code intercepts types named "Foo":
        /// <code>
        ///     Func&lt;TypeReference, bool&gt; filter = 
        ///     concreteType => concreteType.Name == "Foo";
        /// </code>
        /// </remarks>
        public static void InterceptNewInstances(this IReflectionVisitable target, Func<TypeReference, bool> typeFilter,
                                                 Func<MethodReference, bool> methodFilter)
        {
            Func<MethodReference, TypeReference, bool> constructorFilter =
                (constructor, declaringType) => methodFilter(constructor) && typeFilter(declaringType);

            Func<MethodReference, TypeReference, MethodReference, bool> filter =
                (ctor, declaringType, declaringMethod) => constructorFilter(ctor, declaringType) && methodFilter(declaringMethod);

            var redirector = new RedirectNewInstancesToActivator(filter);
            target.InterceptNewInstancesWith(redirector, methodFilter);
        }


        /// <summary>
        /// Modifies a <paramref name="target"/> assembly to support intercepting calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The assembly to be modified.</param>
        /// <param name="constructorFilter">The functor that determines which type instantiations should be intercepted.</param>
        /// <param name="methodFilter">The filter that determines which host methods will be modified</param>
        /// <remarks>
        /// The type filter determines which concrete types and constructors should be intercepted at runtime.
        /// For example, the following functor code intercepts types named "Foo":
        /// <code>
        ///     Func&lt;MethodReference, TypeReference, bool&gt; filter = 
        ///     (constructor, concreteType, hostMethod) => concreteType.Name == "Foo";
        /// </code>
        /// </remarks>
        public static void InterceptNewInstances(this IReflectionStructureVisitable target, Func<MethodReference, TypeReference, bool> constructorFilter,
                                                 Func<MethodReference, bool> methodFilter)
        {
            Func<MethodReference, TypeReference, MethodReference, bool> filter =
                (ctor, declaringType, declaringMethod) => constructorFilter(ctor, declaringType) && methodFilter(declaringMethod);

            var redirector = new RedirectNewInstancesToActivator(filter);
            target.InterceptNewInstancesWith(redirector, methodFilter);
        }

        /// <summary>
        /// Modifies a <paramref name="target"/> assembly to support intercepting calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The assembly to be modified.</param>
        /// <param name="typeFilter">The functor that determines which type instantiations should be intercepted.</param>
        /// <remarks>
        /// The type filter determines the concrete types that should be intercepted at runtime.
        /// For example, the following functor code intercepts types named "Foo":
        /// <code>
        ///     Func&lt;TypeReference, bool&gt; filter = 
        ///     concreteType => concreteType.Name == "Foo";
        /// </code>
        /// </remarks>
        public static void InterceptNewInstances(this IReflectionStructureVisitable target, Func<TypeReference, bool> typeFilter)
        {
            target.InterceptNewInstances(typeFilter, m => true);
        }

        /// <summary>
        /// Modifies a <paramref name="target"/> assembly to support intercepting calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The assembly to be modified.</param>
        /// <param name="typeFilter">The functor that determines which type instantiations should be intercepted.</param>
        /// <remarks>
        /// The type filter determines the concrete types that should be intercepted at runtime.
        /// For example, the following functor code intercepts types named "Foo":
        /// <code>
        ///     Func&lt;TypeReference, bool&gt; filter = 
        ///     concreteType => concreteType.Name == "Foo";
        /// </code>
        /// </remarks>
        public static void InterceptNewInstances(this IReflectionVisitable target, Func<TypeReference, bool> typeFilter)
        {
            target.InterceptNewInstances(typeFilter, m => true);
        }

        /// <summary>
        /// Modifies the <paramref name="target"/> to support intercepting calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The item to be modified.</param>
        /// <param name="methodFilter">The filter that determines which host methods will be modified</param>
        /// <remarks>
        /// The type filter determines which concrete types and constructors should be intercepted at runtime.
        /// For example, the following functor code intercepts types named "Foo":
        /// <code>
        ///     Func&lt;MethodReference, TypeReference, bool&gt; filter = 
        ///     (constructor, concreteType, hostMethod) => concreteType.Name == "Foo";
        /// </code>
        /// </remarks>
        public static void InterceptNewInstances(this IReflectionStructureVisitable target, Func<TypeReference, bool> typeFilter,
                                                 Func<MethodReference, bool> methodFilter)
        {
            Func<MethodReference, TypeReference, bool> constructorFilter =
                (constructor, declaringType) => methodFilter(constructor) && typeFilter(declaringType);

            Func<MethodReference, TypeReference, MethodReference, bool> filter =
                (ctor, declaringType, declaringMethod) => constructorFilter(ctor, declaringType) && methodFilter(declaringMethod);

            var redirector = new RedirectNewInstancesToActivator(filter);
            target.InterceptNewInstancesWith(redirector, methodFilter);
        }


        /// <summary>
        /// Modifies the <paramref name="target"/> to support intercepting calls to the 'new' operator.
        /// </summary>
        /// <param name="target">The item to be modified.</param>
        /// <param name="constructorFilter">The functor that determines which type instantiations should be intercepted.</param>
        /// <param name="methodFilter">The filter that determines which host methods will be modified</param>
        /// <remarks>
        /// The type filter determines which concrete types and constructors should be intercepted at runtime.
        /// For example, the following functor code intercepts types named "Foo":
        /// <code>
        ///     Func&lt;MethodReference, TypeReference, bool&gt; filter = 
        ///     (constructor, concreteType, hostMethod) => concreteType.Name == "Foo";
        /// </code>
        /// </remarks>
        public static void InterceptNewInstances(this IReflectionVisitable target, Func<MethodReference, TypeReference, bool> constructorFilter,
                                                 Func<MethodReference, bool> methodFilter)
        {
            Func<MethodReference, TypeReference, MethodReference, bool> filter =
                (ctor, declaringType, declaringMethod) => constructorFilter(ctor, declaringType) && methodFilter(declaringMethod);

            var redirector = new RedirectNewInstancesToActivator(filter);
            target.InterceptNewInstancesWith(redirector, methodFilter);
        }

        /// <summary>
        /// Modifies the methods in the given <paramref name="target"/> using the custom <see cref="INewObjectWeaver"/> instance.
        /// </summary>
        /// <param name="target">The host that contains the methods that will be modified.</param>
        /// <param name="weaver">The custom <see cref="INewObjectWeaver"/> that will replace all calls to the new operator with the custom code emitted by the given weaver.</param>
        /// <param name="filter">The method filter that will determine which methods should be modified.</param>
        public static void InterceptNewInstancesWith(this IReflectionStructureVisitable target, INewObjectWeaver weaver, Func<MethodReference, bool> filter)
        {
            var interceptNewCalls = new InterceptNewCalls(weaver);
            target.WeaveWith(interceptNewCalls, filter);
        }

        /// <summary>
        /// Modifies the methods in the given <paramref name="target"/> using the custom <see cref="INewObjectWeaver"/> instance.
        /// </summary>
        /// <param name="target">The host that contains the methods that will be modified.</param>
        /// <param name="weaver">The custom <see cref="INewObjectWeaver"/> that will replace all calls to the new operator with the custom code emitted by the given weaver.</param>
        /// <param name="filter">The method filter that will determine which methods should be modified.</param>
        public static void InterceptNewInstancesWith(this IReflectionVisitable target, INewObjectWeaver weaver, Func<MethodReference, bool> filter)
        {
            var interceptNewCalls = new InterceptNewCalls(weaver);
            target.WeaveWith(interceptNewCalls, filter);
        }
    }
}
