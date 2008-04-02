using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;

namespace LinFu.MxClone.Actions
{
    public class CreateInstance : IInstance
    {
        private Type _targetType;
        private object _instance;

        public CreateInstance(Type targetType)
        {
            _targetType = targetType;
        }

        #region IInstance Members

        public object Evaluate()
        {
            if (_targetType == null)
                return null;

            if (_instance == null)
                _instance = Activator.CreateInstance(_targetType);

            return _instance;
        }

        #endregion
    }
}
