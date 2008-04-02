using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone
{
    public class InstanceNotFoundException : Exception
    {
        private readonly string _instanceName;
        public InstanceNotFoundException(string instanceName)
        {
            _instanceName = instanceName;
        }
        public string InstanceName
        {
            get { return _instanceName; }
        }
        public override string Message
        {
            get
            {
                return string.Format("No such instance with name '{0}' found", _instanceName);
            }
        }
    }
}
