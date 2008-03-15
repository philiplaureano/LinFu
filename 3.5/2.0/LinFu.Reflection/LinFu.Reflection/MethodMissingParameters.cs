using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Reflection
{
    public class MethodMissingParameters
    {
        private string _name;
        private object _target;
        private object[] _args;
        private object _returnValue;
        private bool _handled = false;
        public MethodMissingParameters(string name, object target, object[] args)
        {
            _name = name;
            _target = target;
            _args = args;
        }
        public string MethodName
        {
            get { return _name; }
        }

        public object Target
        {
            get { return _target; }
        }

        public object[] Arguments
        {
            get { return _args; }
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public object ReturnValue
        {
            get { return _returnValue; }
            set { _returnValue = value; }
        }
    }
}
