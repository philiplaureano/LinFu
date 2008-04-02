using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Delegates;
using LinFu.Reflection;

namespace LinFu.MxClone.Actions
{
    public class BindToEvent : IAction
    {
        private string _eventName;
        private string _instanceName;
        private string _methodName;
        private IInstance _eventSource;
        private IInstanceHolder _holder;
        public BindToEvent(string eventName, string instanceName, string methodName, IInstance eventSource,
            IInstanceHolder holder)
        {
            _eventName = eventName;
            _instanceName = instanceName;
            _methodName = methodName;
            _eventSource = eventSource;
            _holder = holder;
        }

        #region IAction Members

        public void Execute()
        {
            if (_eventSource == null || _holder == null)
                return;

            object source = _eventSource.Evaluate();

            // Route the event back to the handler
            CustomDelegate handleEvent = (args) =>
            {
                // Obtain a reference to the actual event handler
                object instance = _holder.GetInstance(_instanceName);
                if (instance == null)
                    return null;

                // Call the event handler method
                DynamicObject dynamic = null;

                // HACK: Use it as a dynamic object, if possible
                if (instance is DynamicObject)
                    dynamic = (DynamicObject)instance;

                dynamic = dynamic ?? new DynamicObject(instance);
                // Call the event handler method
                return dynamic.Methods[_methodName](args);
            };

            EventBinder.BindToEvent(_eventName, source, handleEvent);
        }

        #endregion
    }
}
