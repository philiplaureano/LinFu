using System;
using System.Collections.Generic;

namespace LinFu.DynamicProxy
{
    internal struct ProxyCacheEntry
    {
        public Type BaseType;
        public Type[] Interfaces;

        public ProxyCacheEntry(Type baseType, Type[] interfaces)
        {
            BaseType = baseType;
            Interfaces = interfaces;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ProxyCacheEntry))
                return false;

            ProxyCacheEntry other = (ProxyCacheEntry) obj;

            if (other.BaseType != BaseType)
                return false;

            if (Interfaces.Length == 0 && other.Interfaces.Length == 0)
                return true;

            if (Interfaces == null && other.Interfaces != null)
                return false;

            List<Type> interfaceList = new List<Type>(other.Interfaces);

            foreach (Type current in Interfaces)
            {
                if (!interfaceList.Contains(current))
                    return false;
            }

            return true;
        }
    }
}