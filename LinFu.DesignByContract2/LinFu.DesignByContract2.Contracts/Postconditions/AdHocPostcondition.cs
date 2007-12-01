using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;
using LinFu.Reflection;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class AdHocPostcondition<TTarget> : IPostcondition
        where TTarget : class
    {
        private CheckHandler<TTarget> _checker;
        private AppliesToHandler _appliesTo;
        private ShowErrorHandler<TTarget> _showError;
        private readonly Dictionary<string, object> _oldValues = new Dictionary<string, object>();
        private readonly List<string> _saveList = new List<string>();
        public CheckHandler<TTarget> Checker
        {
            get { return _checker; }
            set { _checker = value; }
        }

        public AppliesToHandler AppliesToHandler
        {
            get { return _appliesTo; }
            set { _appliesTo = value; }
        }

        public ShowErrorHandler<TTarget> ShowErrorHandler
        {
            get { return _showError; }
            set { _showError = value; }
        }

        #region IPostcondition Members

        public void BeforeMethodCall(object target, InvocationInfo info)
        {
            if (target == null)
                return;

            // Clear the old values
            _oldValues.Clear();

            // Save the old values
            DynamicObject dynamic = new DynamicObject(target);
            foreach(string propertyName in _saveList)
            {
                _oldValues[propertyName] = dynamic.Properties[propertyName];
            }
        }

        public bool Check(object target, InvocationInfo info, object returnValue)
        {
            if (Checker == null)
                return true;

            TTarget instance = target as TTarget;

            return Checker(instance, info, returnValue);
        }

        public void ShowError(TextWriter output, object target, InvocationInfo info, 
                              object returnValue)
        {
            if (_showError == null)
                return;

            TTarget instance = target as TTarget;
            _showError(output, instance, info, returnValue);
        }

        #endregion

        #region IMethodContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            TTarget instance = target as TTarget;
            if (instance == null)
                return false;
            
            if (_appliesTo == null)
                return false;
            
            return _appliesTo(target, info);
        }
        public void Catch(Exception ex)
        {
            
        }
        #endregion

        internal void SaveProperty(string propertyName)
        {
            if (!_saveList.Contains(propertyName))
                _saveList.Add(propertyName);
        }
        internal object GetOldValue(string propertyName)
        {
            return _oldValues[propertyName];
        }
    }
}
