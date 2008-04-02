using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface IInstanceHolder
    {
        void Register(string instanceName, object instance);
        T GetInstance<T>(string instanceName) where T : class;
        object GetInstance(string instanceName);
    }
}
