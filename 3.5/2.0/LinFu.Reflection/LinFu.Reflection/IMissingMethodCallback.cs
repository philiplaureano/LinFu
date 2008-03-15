using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Reflection
{
    public interface IMethodMissingCallback
    {
        void MethodMissing(object source, MethodMissingParameters missingParameters);

        bool CanHandle(MethodInfo method);
    }
}
