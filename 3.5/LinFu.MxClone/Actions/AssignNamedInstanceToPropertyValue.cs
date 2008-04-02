using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection;

namespace LinFu.MxClone.Actions
{
    public class AssignNamedInstanceToPropertyValue : IAction
    {
        private readonly string _propertyName;
        private readonly string _instanceName;
        private readonly IInstance _target;
        private readonly IInstanceHolder _holder;
        public AssignNamedInstanceToPropertyValue(string propertyName, string instanceName, IInstance target, IInstanceHolder holder)
        {
            _propertyName = propertyName;
            _instanceName = instanceName;
            _target = target;
            _holder = holder;
        }
        #region IAction Members

        public void Execute()
        {
            if (_holder == null)
                return;

            // Use the named instance as the property value
            object propertyValue = _holder.GetInstance(_instanceName);
            if (propertyValue == null)
                throw new InstanceNotFoundException(_instanceName);

            object target = _target.Evaluate();

            // Perform the actual set operation
            DynamicObject dynamic = new DynamicObject(target);
            dynamic.Properties[_propertyName] = propertyValue;
        }

        #endregion
    }
}
