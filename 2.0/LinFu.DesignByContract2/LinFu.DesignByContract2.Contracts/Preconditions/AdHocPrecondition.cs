using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public class AdHocPrecondition<TTarget> : IPrecondition
        where TTarget : class
    {
        private CheckHandler<TTarget> _check;
        private AppliesToHandler _appliesTo;
        private ShowErrorHandler _showError;
        public CheckHandler<TTarget> CheckHandler
        {
            get { return _check; }
            set { _check = value; }
        }

        public AppliesToHandler AppliesToHandler
        {
            get { return _appliesTo; }
            set { _appliesTo = value; }
        }

        public ShowErrorHandler ShowErrorHandler
        {
            get { return _showError; }
            set { _showError = value; }
        }

        #region IPrecondition Members

        public bool Check(object target, InvocationInfo info)
        {
            if (_check == null)
                return true;

            TTarget targetObject = target as TTarget;
            if (targetObject == null || _check == null)
                return true;

            return _check(targetObject, info);
        }

        public void ShowError(TextWriter output, object target, InvocationInfo info)
        {
            TTarget targetObject = target as TTarget;
            if (targetObject == null || _showError == null)
                return;

            _showError(output, targetObject, info);
        }

        #endregion

        #region IMethodContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            TTarget targetObject = target as TTarget;
            if (targetObject == null || _appliesTo == null)
                return false;

            return _appliesTo(targetObject, info);
        }
        public void Catch(Exception ex)
        {

        }
        #endregion
    }
}
