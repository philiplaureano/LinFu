using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;

namespace LinFu.MxClone.Actions
{
    public class RegisterInstanceAction : IAction
    {
        private readonly IInstance _instance;
        private readonly string _name;
        private readonly IInstanceHolder _holder;
        public RegisterInstanceAction(string instanceName, IInstance instance, IInstanceHolder holder)
        {
            _instance = instance;
            _name = instanceName;
            _holder = holder;
        }
        #region IAction Members

        public void Execute()
        {
            if (_holder == null || _instance == null)
                return;

            _holder.Register(_name, _instance.Evaluate());
        }

        #endregion
    }
}
