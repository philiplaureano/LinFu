using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IInitializer
    {
        void InitializeSelf();

        bool CanInitialize(Type targetType);                
        void Initialize(object target);
        void InitializeType(Type targetType);
        void CatchError(Exception ex);
    }
}
