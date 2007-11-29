using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Attributes
{
    internal class CompositePrecondition : IPrecondition
    {
        private List<IPrecondition> _preconditions = new List<IPrecondition>();
        private Queue<IPrecondition> _failedPreconditions = new Queue<IPrecondition>();
        public IList<IPrecondition> Preconditions
        {
            get { return _preconditions;  }   
        }
        #region IPrecondition Members

        public bool Check(object target, InvocationInfo info)
        {
            bool result = true;
            
            foreach(IPrecondition precondition in _preconditions)
            {
                bool currentResult = false;
                
                try
                {
                    currentResult = precondition.Check(target, info);
                }
                catch(Exception ex)
                {
                    if (precondition != null)
                        precondition.Catch(ex);
                }

                if (!precondition.AppliesTo(target, info))
                    continue;
                
                if (currentResult == false)
                    _failedPreconditions.Enqueue(precondition);

                result &= currentResult;
            }
            
            return result;
        }

        public void ShowError(System.IO.TextWriter output, object target, InvocationInfo info)
        {
            while(_failedPreconditions.Count > 0)
            {
                IPrecondition current = _failedPreconditions.Dequeue();
                current.ShowError(output, target, info);
            }
        }

        #endregion

        #region IMethodContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            foreach(IMethodContractCheck check in _preconditions)
            {
                if (check.AppliesTo(target, info))
                    return true;
            }
            
            return false;
        }
        public void Catch(Exception ex)
        {
            
        }
        #endregion
    }
}
