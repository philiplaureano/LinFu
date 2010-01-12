using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace CommonServiceLocator.LinFuAdapter
{
    public sealed class LinFuServiceLocator : ServiceLocatorImplBase
    {
        readonly IServiceContainer _container;

        public LinFuServiceLocator(IServiceContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
                return _container.GetService(key, serviceType);

            // Return the first service if nothing else can be found
            Func<IServiceInfo, bool> criteria = info =>
                {
                    var matchesServiceType = info.ServiceType == serviceType;

                    if (!matchesServiceType)
                        return false;

                    return info.ArgumentTypes == null || info.ArgumentTypes.Count() == 0;
                };

            var defaultService = _container.AvailableServices.Where(criteria).FirstOrDefault();
            return _container.GetService(defaultService);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Func<IServiceInfo, bool> matchesServiceType = info => serviceType == info.ServiceType &&
                                                                  (info.ArgumentTypes == null ||
                                                                   info.ArgumentTypes.Count() == 0);

            var services = _container.AvailableServices.Where(matchesServiceType);
            foreach (var service in services)
            {
                yield return _container.GetService(service);
            }
        }
    }
}
