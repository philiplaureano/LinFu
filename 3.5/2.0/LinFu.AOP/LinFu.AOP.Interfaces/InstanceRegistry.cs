using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public static class InstanceRegistry
    {
        private static readonly List<IInitializer> _initializers = new List<IInitializer>() { new TypeInitializerDecorator(new BootStrapInitializer()) };
        private static readonly object lockObject = new object();
        public static void AddInitializer(IInitializer init)
        {
            if (init == null)
                return;

            lock (lockObject)
            {
                _initializers.Add(new TypeInitializerDecorator(init));
            }
        }
        public static void InitializeType(Type instanceType)
        {
            lock (lockObject)
            {
                var matches = (from i in _initializers
                               where i != null && i.CanInitialize(instanceType)
                               select i).ToList();

                matches.ForEach(i =>
                {
                    try
                    {
                        i.InitializeType(instanceType);
                    }
                    catch (Exception ex)
                    {
                        i.CatchError(ex);
                    }
                });
            }
        }
        public static void InitializeInstance(object instance)
        {
            if (instance == null)
                return;

            Type instanceType = instance.GetType();
            lock (lockObject)
            {
                var matches = (from i in _initializers
                               where i != null && i.CanInitialize(instanceType)
                               select i).ToList();

                matches.ForEach(i =>
                {
                    try
                    {
                        i.Initialize(instance);
                    }
                    catch (Exception ex)
                    {
                        i.CatchError(ex);
                    }
                });
            }
        }
    }
}
