using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Reflection.Extensions
{
    public abstract class BaseCompositeMethodMissingCallback : IMethodMissingCallback
    {
        protected BaseCompositeMethodMissingCallback()
        {
        }

        #region IMethodMissingCallback Members

        public virtual bool CanHandle(MethodInfo method)
        {
            var callbacks = GetCallbacks();
            if (callbacks == null)
                return false;

            var matches = from c in callbacks
                          where c.CanHandle(method)
                          select c;

            return matches.Count() > 0;
        }

        public virtual void MethodMissing(object source, MethodMissingParameters missingParameters)
        {
            var callbacks = GetCallbacks();
            if (callbacks == null)
                return;

            foreach (var callback in callbacks)
            {
                if (missingParameters.Handled == true)
                    return;

                callback.MethodMissing(source, missingParameters);
            }
        }

        #endregion
        protected abstract IEnumerable<IMethodMissingCallback> GetCallbacks();
    }
}
