using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface IFactory<T>
    {
        T CreateInstance(IContainer container);
    }
}
