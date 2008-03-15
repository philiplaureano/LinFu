using System;
using System.Collections.Generic;

namespace Simple.IoC
{
    public interface IContainer
    {
        void AddService<T>(T serviceInstance);
        void AddFactory<T>(IFactory<T> factory);
        void AddFactory(Type itemType, IFactory factory);

        bool Contains(Type serviceType);
        T GetService<T>() where T : class;
        T GetService<T>(string serviceName) where T : class;

        INamedFactoryStorage NamedFactoryStorage { get; set; }

        IList<ICustomizeInstance> Customizers { get; }
        IList<IPropertyInjector> PropertyInjectors { get; }
        IList<ITypeInjector> TypeInjectors { get; }
        IList<ITypeSurrogate> TypeSurrogates { get; }
    }
}