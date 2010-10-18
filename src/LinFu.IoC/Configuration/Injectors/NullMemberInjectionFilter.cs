using System;
using System.Collections.Generic;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration.Injectors
{
    /// <summary>
    /// Represents a type of <see cref="IMemberInjectionFilter{TMember}"/>
    /// that always returns an empty match.
    /// </summary>
    public class NullMemberInjectionFilter<TMember> : IMemberInjectionFilter<TMember>
        where TMember : MemberInfo
    {
        #region IMemberInjectionFilter<TMember> Members

        /// <summary>
        /// Always returns an empty list of injectable members.
        /// </summary>
        /// <param name="targetType">The type to be injected.</param>
        /// <returns>An empty list.</returns>
        public IEnumerable<TMember> GetInjectableMembers(Type targetType)
        {
            return new TMember[0];
        }

        #endregion
    }
}