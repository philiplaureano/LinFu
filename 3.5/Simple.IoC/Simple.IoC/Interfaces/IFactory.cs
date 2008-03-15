using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface IFactory
    {
        object CreateInstance(IContainer container);
    }
}
