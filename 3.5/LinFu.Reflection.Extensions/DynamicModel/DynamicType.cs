using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public partial class DynamicType : BaseCompositeMethodMissingCallback
    {
        private readonly DynamicObject _typeRepresentation = new DynamicObject();
        public static DynamicType operator +(DynamicType lhs, DynamicType rhs)
        {
            return new CompositeDynamicType(new DynamicType[] { lhs, rhs });
        }
        public DynamicType()
        {
            _typeRepresentation += this;
        }
        public DynamicType(TypeSpec typeSpec)
        {
            _typeRepresentation += this;
            TypeSpec = typeSpec;
        }
        public TypeSpec TypeSpec
        {
            get;
            set;
        }
        public bool LooksLike<T>()
            where T : class
        {
            // HACK: Make the type representation (a dynamic object) act like the current dynamic type
            // and use it to determine if it looks like the target interface
            return _typeRepresentation.LooksLike<T>();
        }
        public bool LooksLike(Type targetType)
        {
            return _typeRepresentation.LooksLike(targetType);
        }
        protected override IEnumerable<IMethodMissingCallback> GetCallbacks()
        {
            if (TypeSpec == null)
                return new IMethodMissingCallback[0];

            List<IMethodMissingCallback> results = new List<IMethodMissingCallback>();

            foreach (var property in TypeSpec.Properties)
            {
                var newProperty = new DynamicProperty() { PropertySpec = property };
                results.Add(newProperty);
            }

            foreach (var method in TypeSpec.Methods)
            {
                var newMethod = new DynamicMethod(method);
                results.Add(newMethod);
            }

            return results;
        }
    }
}
