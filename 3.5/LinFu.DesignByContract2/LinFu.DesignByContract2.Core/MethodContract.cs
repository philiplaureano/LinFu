using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Core
{
    public class MethodContract : IMethodContract
    {
        private List<IPrecondition> _preconditions = new List<IPrecondition>();
        private List<IPostcondition> _postconditions = new List<IPostcondition>();

        public IList<IPrecondition> Preconditions
        {
            get { return _preconditions; }
        }

        public IList<IPostcondition> Postconditions
        {
            get { return _postconditions; }
        }
    }
}
