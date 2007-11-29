using System;
using System.IO;
using System.Reflection;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public class ContractChecker : IContractChecker
    {
        private IContractProvider _provider;
        private IErrorView _errorView;
        private bool _checkPreconditions = true;
        private bool _checkPostconditions = true;
        private bool _checkInvariants = true;
        
        private object _target;
        protected ContractChecker()
        {            
        }
        public ContractChecker(IContractProvider provider)
        {
            _provider = provider;
        }

        public object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public IErrorView ErrorView
        {
            get { return _errorView; }
            set { _errorView = value; }
        }

        public IContractProvider ContractProvider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public bool CheckPreconditions
        {
            get { return _checkPreconditions; }
            set { _checkPreconditions = value; }
        }

        public bool CheckPostconditions
        {
            get { return _checkPostconditions; }
            set { _checkPostconditions = value; }
        }

        public bool CheckInvariants
        {
            get { return _checkInvariants; }
            set { _checkInvariants = value; }
        }

        public void BeforeInvoke(InvocationInfo info)
        {
            if (_provider == null)
                return;
            Type targetType = _target.GetType();
            ITypeContract typeContract = _provider.GetTypeContract(targetType);
            
            if (typeContract == null)
                throw new NullReferenceException("The contract provider must return a valid type contract.");

            IMethodContract methodContract = _provider.GetMethodContract(targetType, info);
            if (methodContract == null)
                throw new NullReferenceException("The contract provider must return a valid method contract.");
            
            // HACK: Show the state of the object to the postconditions
            // to emulate eiffel's 'old' operator syntax
            foreach (IPostcondition postcondition in methodContract.Postconditions)
            {
                if (!_checkPostconditions || !postcondition.AppliesTo(_target, info))
                    continue;

                try
                {
                    postcondition.BeforeMethodCall(_target, info);    
                }
                catch(Exception ex)
                {
                    // Reflect the exception back to the postcondition
                    postcondition.Catch(ex);
                }
            }

            CallInvariants(info, InvariantState.BeforeMethodCall);

            #region Call the preconditions
            bool preconditionResult = false;

            StringWriter output = new StringWriter();
            foreach (IPrecondition precondition in methodContract.Preconditions)
            {
                if (!_checkPreconditions || !precondition.AppliesTo(_target, info))
                    continue;
                
                
                bool currentResult = false;

                // Reflect any exceptions thrown by the precondition
                // back to the current check itself
                try
                {
                    currentResult = precondition.Check(_target, info);                    
                }
                catch(Exception ex)
                {
                    precondition.Catch(ex);
                }

                preconditionResult |= currentResult;
                if (currentResult == true)
                    continue;

                StringWriter writer = new StringWriter();
                precondition.ShowError(writer, _target, info);
                writer.WriteLine();

                output.Write(writer.ToString());
                if (_errorView == null)
                    continue;

                _errorView.ShowPreconditionError(writer.ToString());
            }            
            if (!preconditionResult && methodContract.Preconditions.Count > 0)
                throw new PreconditionViolationException(output.ToString(), info);

            #endregion
        }

        public object DoInvoke(InvocationInfo info)
        {            
            MethodInfo targetMethod = info.TargetMethod;
                        
            if (targetMethod.IsGenericMethodDefinition)
            {
                // Apply the type parameters to the generic method definition
                // to get the correct method
                targetMethod = targetMethod.MakeGenericMethod(info.TypeArguments); 
            }
            
            return targetMethod.Invoke(_target, info.Arguments);
        }

        public void AfterInvoke(InvocationInfo info, object returnValue)
        {

            Type targetType = _target.GetType();
            IMethodContract methodContract = _provider.GetMethodContract(targetType, info);
            if (methodContract == null)
                return;
            
            #region Call the postconditions
            bool postconditionResult = true;
            StringWriter writer = new StringWriter();
            foreach (IPostcondition postcondition in methodContract.Postconditions)
            {
                if (!_checkPostconditions || !postcondition.AppliesTo(_target, info))
                    continue;
                
                // Check the return value
                try
                {
                    postconditionResult &= postcondition.Check(_target, info, returnValue);    
                }
                catch(Exception ex)
                {
                    // Reflect any exceptions thrown by the postcondition
                    // back to itself
                    postcondition.Catch(ex);
                }
                
                if (postconditionResult == true)
                    continue;

                // Display any errors that might occur
                postcondition.ShowError(writer, _target, info, returnValue);

                if (_errorView != null)
                    _errorView.ShowPostconditionError(writer.ToString());

                break;
            }

            if (postconditionResult == false)
                throw new PostconditionViolationException(writer.ToString(), info);
            #endregion 

            ITypeContract typeContract = _provider.GetTypeContract(targetType);

            if (typeContract == null)
                return;
            
            CallInvariants(info, InvariantState.AfterMethodCall);
        }

        private void CallInvariants(InvocationInfo info, InvariantState state)
        {
            if (!_checkInvariants || _provider == null)
                return;
            
            Type targetType = _target.GetType();
            ITypeContract contract = _provider.GetTypeContract(targetType);
            if (contract == null)
                return;

            bool checkResult = true;

            StringWriter writer = new StringWriter();
            foreach (IInvariant invariant in contract.Invariants)
            {
                // Match each invariant to the correct type
                if (!invariant.AppliesTo(_target, info))
                    continue;
                
                // Verify the object state
                try
                {
                    checkResult &= invariant.Check(_target, info, state);    
                }
                catch(Exception ex)
                {
                    // Reflect any exceptions thrown by the postcondition
                    // back to itself
                    invariant.Catch(ex);
                }

                if (checkResult)
                    continue;

                // Display any errors that might occur
                invariant.ShowError(writer, _target, info, state);

                if (_errorView != null)
                    _errorView.ShowInvariantError(writer.ToString());

                break;
            }

            if (checkResult == false)            
                throw new InvariantViolationException(writer.ToString(), info);
            
        }
    }
}