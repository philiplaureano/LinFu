using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    [Serializable]
    public class CompositeTypeSpec : TypeSpec
    {
        private List<MethodSpec> _methodList;
        private List<PropertySpec> _propertyList;
        public CompositeTypeSpec(params TypeSpec[] specs)
        {
            _methodList = (from s in specs
                           from m in s.Methods
                           select m).ToList();


            _propertyList = (from s in specs
                             from p in s.Properties
                             select p).ToList();

        }
        public override List<MethodSpec> Methods
        {
            get
            {
                return _methodList;
            }
        }
        public override List<PropertySpec> Properties
        {
            get
            {
                return _propertyList;
            }
        }
    }
}
