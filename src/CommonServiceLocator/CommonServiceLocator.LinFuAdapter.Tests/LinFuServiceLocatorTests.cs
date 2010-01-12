using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonServiceLocator.LinFuAdapter.Components;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using NUnit.Framework;

namespace CommonServiceLocator.LinFuAdapter.Tests
{
    [TestFixture]
    public class LinFuServiceLocatorTests : ServiceLocatorTestCase
    {
        protected override Microsoft.Practices.ServiceLocation.IServiceLocator CreateServiceLocator()
        {
            var container = new ServiceContainer();
            container.LoadFromBaseDirectory("*.dll");

            container.AddService(typeof (SimpleLogger).FullName, typeof (ILogger), typeof (SimpleLogger),
                                 LifecycleType.Singleton);

            container.AddService(typeof(AdvancedLogger).FullName, typeof(ILogger), typeof(AdvancedLogger),
                                 LifecycleType.Singleton);

            return new LinFuServiceLocator(container);
        }
    }
}
