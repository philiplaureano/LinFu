using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class responsible for setting property values.
    /// </summary>
    [Implements(typeof(IPropertySetter), LifecycleType.OncePerRequest)]
    public class PropertySetter : IPropertySetter
    {
        private static readonly Dictionary<PropertyInfo, Action<object, object>> _setters =
            new Dictionary<PropertyInfo, Action<object, object>>();

        private static readonly Type[] _parameterTypes = new Type[] { typeof(object), typeof(object) };

        /// <summary>
        /// Sets the value of the <paramref name="targetProperty"/>.
        /// </summary>
        /// <param name="target">The target instance that contains the property to be modified.</param>
        /// <param name="targetProperty">The property that will store the given value.</param>
        /// <param name="value">The value that will be assigned to the property.</param>
        public void Set(object target, PropertyInfo targetProperty, object value)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            // Reuse the cached results, if possible
            Action<object, object> setter = null;
            if (_setters.ContainsKey(targetProperty))
            {
                setter = _setters[targetProperty];
                setter(target, value);
                return;
            }

            setter = GenerateSetter(targetProperty);

            lock(_setters)
            {
                _setters[targetProperty] = setter;
            }

            if (setter != null)
                setter(target, value);
        }

        /// <summary>
        /// Generates an <see cref="Action{T1, T2}"/> delegate that will be used
        /// as the property setter for a particular type.
        /// </summary>
        /// <param name="targetProperty">The property that will be modified.</param>
        /// <returns>A property setter.</returns>
        private static Action<object, object> GenerateSetter(PropertyInfo targetProperty)
        {
            var setterMethod = targetProperty.GetSetMethod();

            if (setterMethod == null)
                throw new ArgumentException(string.Format("The property '{0}' is missing a setter method!", targetProperty));

            // Validate the setter method
            if (!setterMethod.IsPublic)
                throw new ArgumentException(
                    string.Format("The property '{0}' must have a publicly visible setter in order to be modified", targetProperty));

            // HACK: Manually invoke the setter since the Mono runtime currently 
            // does not support the DynamicMethod class
            if (Runtime.IsRunningOnMono)
                return (target, value) => setterMethod.Invoke(target, new object[] {value});
            
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(void), _parameterTypes);
            var IL = dynamicMethod.GetILGenerator();
            
            // Push the target instance onto the stack
            IL.Emit(OpCodes.Ldarg_0);

            // Cast it to the appropriate type
            IL.Emit(OpCodes.Isinst, targetProperty.DeclaringType);
            
            // NOTE: A null reference check was intentionally omitted to make sure that the program
            // crashes if the instance type is of the wrong type

            // Push the setter value
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Isinst, targetProperty.PropertyType);

            // Call the setter
            var callInstruction = setterMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
            IL.Emit(callInstruction, setterMethod);
            IL.Emit(OpCodes.Ret);

            var setter = (Action<object, object>) dynamicMethod.CreateDelegate(typeof(Action<object, object>));
            return setter;
        }
    }
}
