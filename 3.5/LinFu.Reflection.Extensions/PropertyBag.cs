using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Reflection.Extensions
{
    public class PropertyBag : IPropertyBehavior
    {
        private readonly Dictionary<int, object> _values = new Dictionary<int, object>();
        public IPropertyTypingStrategy TypingStrategy { get; set; }
        
        public PropertyBag()
        {

        }

        public object GetValue(object target, string propertyName, Type propertyType)
        {
            if (TypingStrategy != null &&
                !TypingStrategy.PropertyMatches(propertyName, propertyType))
                return null;

            // Hack: Maintain a weak reference to the target
            // by using its hashcode
            int hashCode = target.GetHashCode();
            if (!_values.ContainsKey(hashCode))
                return GetDefaultValue(propertyType);

            return _values[hashCode];
        }

        public void SetValue(object target, string propertyName, Type propertyType, object value)
        {
            if (TypingStrategy != null &&
                !TypingStrategy.PropertyMatches(propertyName, propertyType))
            {
                var targetTypeName = target != null ? target.GetType().AssemblyQualifiedName :
                    "(null)";

                var format = "The property named '{0}' on target type '{1}' must be convertible to '{2}'";
                var message = string.Format(format, propertyName, targetTypeName, propertyType.AssemblyQualifiedName);
                throw new InvalidOperationException(message);
            }
            // Hack: Maintain a weak reference to the target
            // by using its hashcode
            int hashCode = target.GetHashCode();

            _values[hashCode] = value;
        }

        private object GetDefaultValue(Type targetType)
        {
            var getDefaultMethodDefinition = typeof(PropertyBag).GetMethod("Default", 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            var getDefaultMethod = getDefaultMethodDefinition.MakeGenericMethod(targetType);

            return getDefaultMethod.Invoke(null, new object[0]);
        }
        internal static T Default<T>()
        {
            return default(T);
        }
    }
}
