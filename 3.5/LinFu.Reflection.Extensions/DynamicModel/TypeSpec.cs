using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simple.IoC;
namespace LinFu.Reflection.Extensions
{
    [Serializable]
    public class TypeSpec
    {
        private readonly List<PropertySpec> _properties = new List<PropertySpec>();
        private readonly List<MethodSpec> _methods = new List<MethodSpec>();

        public static implicit operator DynamicType(TypeSpec other)
        {
            return new DynamicType(other);
        }
        public static TypeSpec operator +(TypeSpec lhs, TypeSpec rhs)
        {
            return new CompositeTypeSpec(lhs, rhs);
        }
        public TypeSpec()
        {
        }
        public virtual string Name
        {
            get;
            set;
        }
        public virtual List<PropertySpec> Properties
        {
            get { return _properties; }
        }
        public virtual List<MethodSpec> Methods
        {
            get { return _methods; }
        }
    }
}
