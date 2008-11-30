using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using LinFu.Proxy.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// The base class that defines the behavior for automatically injecting service
    /// instances into type member instances.
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    public abstract class AutoMemberInjector<TMember> : IPostProcessor, IContainerPlugin
        where TMember : MemberInfo
    {
        private static readonly HashSet<Type> _excludedServices = new HashSet<Type>(new
            Type[] { typeof(IMemberInjectionFilter<TMember>), typeof(IArgumentResolver), typeof(IPropertySetter) });

        private bool _inProcess = false;

        /// <summary>
        /// Automatically injects service instances
        /// into properties as soon as they are initialized.
        /// </summary>
        /// <param name="result">The service request result that contains the service whose members will be injected with service instances.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            // Prevent recursion
            if (_inProcess)
                return;

            lock (this)
            {
                _inProcess = true;
            }

            AutoInject(result);

            lock (this)
            {
                _inProcess = false;
            }
        }

        /// <summary>
        /// Injects services from the <paramref name="container"/> into the target <paramref name="member"/> instance.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="member">The <typeparamref name="TMember"/> instance that will store the service instance.</param>
        /// <param name="argumentResolver">The <see cref="IArgumentResolver"/> that will determine which arguments will be assigned to the target member.</param>
        /// <param name="additionalArguments">The additional arguments that were passed to the <see cref="IServiceRequestResult"/> during the instantiation process.</param>
        /// <param name="container">The container that will provide the service instances.</param>
        protected abstract void Inject(object target, TMember member, IArgumentResolver argumentResolver, 
            IServiceContainer container, object[] additionalArguments);

        /// <summary>
        /// Injects a member service dependency into a target service instance.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> that will be processed for injection.</param>
        private void AutoInject(IServiceRequestResult result)
        {
            // Ignore the excluded services
            if (_excludedServices.Contains(result.ServiceType))
                return;

            if (result.ServiceType.IsGenericType)
            {
                var baseDefinition = result.ServiceType.GetGenericTypeDefinition();
                if (baseDefinition == typeof(IMemberInjectionFilter<>))
                    return;
            }

            var container = result.Container;

            if (!container.Contains(typeof(IMemberInjectionFilter<TMember>)))
                return;

            var filter = container.GetService<IMemberInjectionFilter<TMember>>();
            if (filter == null || result.ActualResult == null)
                return;

            // Determine which members can be injected
            var targetType = result.ActualResult.GetType();

            // Use the base class if the
            // target type is a proxy type
            if (typeof(IProxy).IsAssignableFrom(targetType) && targetType.BaseType != typeof(object))
            {
                targetType = targetType.BaseType;
            }

            var members = filter.GetInjectableMembers(targetType).ToList();
            if (members.Count == 0)
                return;

            // Use the resolver to determine
            // which value should be injected
            // into the member
            var resolver = container.GetService<IArgumentResolver>();
            if (resolver == null)
                return;

            var target = result.ActualResult;
            foreach (var member in members)
            {
                Inject(target, member, resolver, container, result.AdditionalArguments);
            }
        }

        
        /// <summary>
        /// Does absolutely nothing.
        /// </summary>
        /// <param name="target">The target container.</param>
        public void BeginLoad(IServiceContainer target)
        {
        }

        /// <summary>
        /// Inserts the <see cref="AutoPropertyInjector"/> class at the end
        /// of the PostProcessor chain.
        /// </summary>
        /// <param name="target">The target container.</param>
        public void EndLoad(IServiceContainer target)
        {
            target.PostProcessors.Add(this);
        }
    }
}
