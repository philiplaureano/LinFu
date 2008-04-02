using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Reflection.Extensions
{
    internal class PropertySetterCallback : IMethodMissingCallback
    {
        public PropertySetterCallback()
        {

        }
        public PropertySetterCallback(PropertySpec propertySpec)
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
            string setterName = string.Format("set_{0}", PropertySpec.PropertyName);

            var aliasMatches = (from alias in PropertySpec.Aliases
                                let currentMethodName = string.Format("set_{0}", alias)
                                where currentMethodName == methodName
                                select alias).Count();

            if (methodName != setterName && aliasMatches == 0)
                return false;

            // The method should have one parameter that matches
            // the current property type
            var parameters = method.GetParameters();
            if (parameters == null || parameters.Length != 1)
                return false;

            var loneParameter = parameters[0];

            if (loneParameter.ParameterType != PropertySpec.PropertyType)
                return false;

            return true;
        }

        public void MethodMissing(object source, MethodMissingParameters missingParameters)
        {
            if (PropertySpec == null)
                return;

            var arguments = missingParameters.Arguments;
            if (arguments.Length != 1)
                return;

            string setterName = string.Format("set_{0}", PropertySpec.PropertyName);

            var aliasMatches = (from alias in PropertySpec.Aliases
                                let currentMethodName = string.Format("set_{0}", alias)
                                where currentMethodName == missingParameters.MethodName
                                select alias).Count();

            if (missingParameters.MethodName != setterName &&
                aliasMatches == 0)
                return;

            // Match the runtime argument type
            Type argumentType = arguments[0] == null ? typeof(void) : arguments[0].GetType();

            if (argumentType != PropertySpec.PropertyType)
                return;

            if (!PropertySpec.CanWrite)
                throw new InvalidOperationException(string.Format("The property '{0}' does not allow writes", PropertySpec.PropertyName));

            // Set the property value
            var behavior = PropertySpec.Behavior;
            behavior.SetValue(source, PropertySpec.PropertyName, PropertySpec.PropertyType, arguments[0]);

            missingParameters.Handled = true;
        }

        #endregion
    }
}
