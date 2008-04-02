using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public static class TypeSpecExtensions
    {
        public static bool LooksLike<T>(this TypeSpec spec)
            where T : class
        {
            DynamicType dynamic = new DynamicType(spec);
            return dynamic.LooksLike<T>();
        }
        public static bool LooksLike(this TypeSpec spec, Type targetType)
        {
            DynamicType dynamicType = new DynamicType(spec);

            return dynamicType.LooksLike(targetType);
        }
        public static T CreateDuck<T>(this TypeSpec spec)
            where T : class
        {
            DynamicType type = new DynamicType(spec);
            DynamicObject dynamic = new DynamicObject();
            dynamic += type;

            return dynamic.CreateDuck<T>();
        }
        public static object CreateDuck(this TypeSpec spec, Type targetType)
        {
            DynamicType type = new DynamicType(spec);
            DynamicObject dynamic = new DynamicObject();
            dynamic += type;

            return dynamic.CreateDuck(targetType);
        }
        public static void AddProperty(this TypeSpec spec, string propertyName,
            Type propertyType)
        {
            PropertySpec property = new PropertySpec();
            property.PropertyName = propertyName;

            PropertyBag bag = new PropertyBag();
            property.PropertyType = propertyType;
            spec.Properties.Add(property);
        }
    }
}
