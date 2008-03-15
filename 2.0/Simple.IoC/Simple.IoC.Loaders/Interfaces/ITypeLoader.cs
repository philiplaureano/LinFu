using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Simple.IoC
{
    public interface ITypeLoader
    {
        Type[] LoadTypes(Assembly assembly);
    }
}
