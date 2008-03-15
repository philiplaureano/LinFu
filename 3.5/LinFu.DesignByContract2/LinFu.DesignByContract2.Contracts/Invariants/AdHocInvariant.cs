using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public class AdHocInvariant<T> : IInvariant
        where T : class
    {
        private AppliesToHandler _appliesToHandler;
        private CheckHandler<T> _checkHandler;
        private ShowErrorHandler _showErrorHandler;

        public AppliesToHandler AppliesToHandler
        {
            get { return _appliesToHandler; }
            set { _appliesToHandler = value; }
        }

        public CheckHandler<T> CheckHandler
        {
            get { return _checkHandler; }
            set { _checkHandler = value; }
        }

        public ShowErrorHandler ShowErrorHandler
        {
            get { return _showErrorHandler; }
            set { _showErrorHandler = value; }
        }

        public bool AppliesTo(object target, InvocationInfo info)
        {
            if (_appliesToHandler == null)
                throw new NotImplementedException();

            return _appliesToHandler(target, info);
        }

        public bool Check(object target, InvocationInfo info, InvariantState callState)
        {
            if (_checkHandler == null)
                return true;

            return _checkHandler(target as T, info, callState);
        }

        public void ShowError(TextWriter output, object target, InvocationInfo info, InvariantState callState)
        {
            throw new NotImplementedException();
        }
        public void Catch(Exception ex)
        {

        }
    }
}
