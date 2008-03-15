using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface IFactoryLoader
    {
        void LoadFactory(IContainer container, Type loadedType);
    }
}
