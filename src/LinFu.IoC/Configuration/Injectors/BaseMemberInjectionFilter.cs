using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Defines the basic behavior of the <see cref="IMemberInjectionFilter{TMember}"/> interface.
    /// </summary>
    /// <typeparam name="TMember">The member type that will be filtered.</typeparam>
    public abstract class BaseMemberInjectionFilter<TMember> : IMemberInjectionFilter<TMember>, IInitialize
        where TMember : MemberInfo 
    {
        private static readonly Dictionary<Type, IEnumerable<TMember>> _itemCache =
            new Dictionary<Type, IEnumerable<TMember>>();

        private IServiceContainer _container;

        /// <summary>
        /// Returns the list of <typeparamref name="TMember"/> objects
        /// whose setters will injected with arbitrary values.
        /// </summary>
        /// <remarks>This implementation selects properties that are marked with the <see cref="InjectAttribute"/>.</remarks>
        /// <param name="targetType">The target type that contains the target properties.</param>
        /// <returns>A set of properties that describe which parameters should be injected.</returns>
        public virtual IEnumerable<TMember> GetInjectableMembers(Type targetType)
        {
            IEnumerable<TMember> items = null;

            // Retrieve the property list only once
            if (!_itemCache.ContainsKey(targetType))
            {
                // The property must have a getter and the current type
                // must exist as either a service list or exist as an 
                // existing service inside the current container
                var members = from item in GetMembers(targetType, _container)                             
                             select item;

                lock (_itemCache)
                {
                    _itemCache[targetType] = members;
                }
            }

            items = _itemCache[targetType];

            return Filter(_container, items);
        }

        /// <summary>
        /// Determines which members should be selected from the <paramref name="targetType"/>
        /// using the <paramref name="container"/>
        /// </summary>
        /// <param name="targetType">The target type that will supply the list of members that will be filtered.</param>
        /// <param name="container">The target container.</param>
        /// <returns>A list of <typeparamref name="TMember"/> objects that pass the filter description.</returns>
        protected abstract IEnumerable<TMember> GetMembers(Type targetType, IServiceContainer container);

        /// <summary>
        /// Determines which items should be injected from the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="container">The source container that will supply the values for the selected members.</param>
        /// <param name="items">The list of properties that will be filtered.</param>
        /// <returns>A list of properties that will be injected.</returns>
        protected virtual IEnumerable<TMember> Filter(IServiceContainer container,
                                                            IEnumerable<TMember> items)
        {
            return items;
        }

        /// <summary>
        /// Initializes the <see cref="BaseMemberInjectionFilter{TMember}"/> class.
        /// </summary>
        /// <param name="source">The host container.</param>
        public virtual void Initialize(IServiceContainer source)
        {
            _container = source;
        }
    }
}