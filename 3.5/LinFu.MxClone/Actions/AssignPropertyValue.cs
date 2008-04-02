using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace LinFu.MxClone.Actions
{
    public class AssignPropertyValue : IAction
    {
        private IInstance _target;
        private IParserContext _context;
        private string _value;
        private string _propertyName;
        
        public AssignPropertyValue(string propertyName, string propertyValue, 
            IInstance target, IParserContext context)
        {
            _target = target;
            _value = propertyValue;
            _propertyName = propertyName;
            _context = context;
        }

        #region IAction Members

        public void Execute()
        {
            if (_target == null)
                return;

            object instance = _target.Evaluate();
            if (instance == null)
                return;

            DynamicObject dynamic = new DynamicObject(instance);

            // Convert the to the property type, if necessary
            Type targetType = instance.GetType();
            PropertyInfo targetProperty = targetType.GetProperty(_propertyName);
            if (targetProperty == null)
                return;

            object propertyValue = _value;

            Type propertyType = targetProperty.PropertyType;
            if (propertyType == typeof(string))
            {
                dynamic.Properties[_propertyName] = propertyValue;
                return;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(propertyType);            
            bool isConvertible = converter.CanConvertFrom(typeof(string));

            if (!isConvertible)
                propertyValue = AttemptConversion(targetType, propertyType, propertyValue);
            else
                propertyValue = converter.ConvertFromString(null, CultureInfo.InvariantCulture, _value);


            // Perform the actual conversion            
            dynamic.Properties[_propertyName] = propertyValue;
        }

        private object AttemptConversion(Type targetType, Type propertyType, object propertyValue)
        {
            // Find a replacement converter, if possible
            var descriptor = _context.TypeDescriptor;
            ICustomTypeConverter customConverter = null;

            if (descriptor != null)
                customConverter = descriptor.GetConverter(propertyType);

            if (customConverter != null)
                return customConverter.ConvertFromString(_value);

            string errorMessage = string.Format("Unable to convert from string to type '{0}' on type '{1}', property name = '{2}",
                propertyType.AssemblyQualifiedName,
                targetType.AssemblyQualifiedName,
                _propertyName);

            throw new InvalidOperationException(errorMessage);
        }

        #endregion
    }
}
