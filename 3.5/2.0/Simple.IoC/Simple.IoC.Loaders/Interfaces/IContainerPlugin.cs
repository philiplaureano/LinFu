using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Loaders
{
    public interface IContainerPlugin
    {
        void BeginLoad(IContainer container);
        void EndLoad(IContainer container);
    }
}
