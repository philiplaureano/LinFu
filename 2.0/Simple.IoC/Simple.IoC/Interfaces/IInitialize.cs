using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public interface IInitialize
    {
        void Initialize(IContainer container);
    }
}
