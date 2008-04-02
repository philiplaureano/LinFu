using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using System.Reflection;
using System.Collections;

namespace LinFu.MxClone.Indexes
{
    public class PropertyIndex : IPropertyIndex
    {
        private readonly Type _targetType;
        private readonly Dictionary<string, PropertyInfo> _index = new Dictionary<string, PropertyInfo>();
        private readonly Dictionary<string, bool> _isCollection = new Dictionary<string, bool>();
        public PropertyIndex(Type targetType)
        {
            _targetType = targetType;
            CreateIndexFrom(targetType);
        }
        public bool HasProperty(string propertyName)
        {
            return _index.ContainsKey(propertyName);
        }
        public bool HasCollectionProperty(string propertyName)
        {
            return HasProperty(propertyName) && _isCollection[propertyName];
        }
        public Type GetPropertyType(string propertyName)
        {
            if (!_index.ContainsKey(propertyName))
                throw new ArgumentException(string.Format("Property '{0}' not found on type '{1}'", propertyName, _targetType.AssemblyQualifiedName));

            return _index[propertyName].PropertyType;
        }
        private void CreateIndexFrom(Type targetType)
        {
            foreach (var property in targetType.GetProperties())
            {
                Index(property);
            }
        }

        private void Index(PropertyInfo property)
        {
            bool isCollection = false;

            Type propertyType = property.PropertyType;

            if (typeof(ICollection).IsAssignableFrom(propertyType))
                isCollection = true;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IList<>))
                isCollection = true;

            _isCollection[property.Name] = isCollection;
            _index[property.Name] = property;
        }
    }
}
