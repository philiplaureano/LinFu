using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Reflection.Extensions
{
    internal class PropertyGetterCallback : IMethodMissingCallback
    {
        public PropertyGetterCallback(PropertySpec propertySpec)
        {
            PropertySpec = propertySpec;
        }
        public PropertySpec PropertySpec
        {
            get;
            set;
        }
        #region IMethodMissingCallback Members

        public bool CanHandle(MethodInfo method)
        {
            if (PropertySpec == null)
                return false;

            string methodName = method.Name;
            string getterName = string.Format("get_{0}", PropertySpec.PropertyName);

            var aliasMatches = (from alias in PropertySpec.Aliases
                                let currentMethodName = string.Format("get_{0}", alias)
                                where currentMethodName == methodName
                                select alias).Count();

            if (methodName != getterName && aliasMatches == 0)
                return false;

            // The return type must match the property type
            if (method.ReturnType != PropertySpec.PropertyType)
                return false;

            return true;
        }

        public void MethodMissing(object source, MethodMissingParameters missingParameters)
        {
            if (PropertySpec == null)
                return;

            string methodName = missingParameters.MethodName;

            // Search for the getter
            var aliasMatches = (from alias in PropertySpec.Aliases
                                let currentMethodName = string.Format("get_{0}", alias)
                                where currentMethodName == methodName
                                select alias).Count();

            string getterName = string.Format("get_{0}", PropertySpec.PropertyName);
            if (methodName != getterName && aliasMatches == 0)
                return;

            if (!PropertySpec.CanRead)
                throw new InvalidOperationException(string.Format("The property '{0}' does not allow reads", PropertySpec.PropertyName));

            var behavior = PropertySpec.Behavior;
            missingParameters.ReturnValue = behavior.GetValue(source, PropertySpec.PropertyName, PropertySpec.PropertyType);
            missingParameters.Handled = true;
        }

        #endregion
    }
}
