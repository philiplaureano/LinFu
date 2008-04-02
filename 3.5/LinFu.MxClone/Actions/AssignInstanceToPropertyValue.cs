using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Actions;
using Simple.IoC.Extensions;
using LinFu.Reflection;

namespace LinFu.MxClone.Actions
{
    public class AssignInstanceToPropertyValue : IAction
    {
        private readonly string _propertyName;
        private readonly IInstance _instanceToAssign;
        private readonly IInstance _target;
        public AssignInstanceToPropertyValue(string propertyName, IInstance instanceToAssign,
            IInstance target)
        {
            _propertyName = propertyName;
            _instanceToAssign = instanceToAssign;
            _target = target;
        }
        #region IAction Members

        public void Execute()
        {
            if (_instanceToAssign == null)
                return;

            object propertyValue = _instanceToAssign.Evaluate();
            object target = _target.Evaluate();

            // Perform the actual set operation
            DynamicObject dynamic = new DynamicObject(target);
            dynamic.Properties[_propertyName] = propertyValue;
        }

        #endregion
    }
}
