using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class AdHocPostcondition<TTarget> : IPostcondition
        where TTarget : class
    {
        private CheckHandler<TTarget> _checker;
        private AppliesToHandler _appliesTo;
        private ShowErrorHandler<TTarget> _showError;
        
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
            // TODO: Save the object state here
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
    }
}
