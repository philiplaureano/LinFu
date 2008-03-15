using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public class PropertyNameFilter<T>
    {
        private string _propertyName;
        private IMethodContract _contract;
        public PropertyNameFilter(string propertyName, IMethodContract contract)
        {
            _propertyName = propertyName;
            _contract = contract;
        }

        public PropertyComparison<T> ComparedToOldValue
        {
            get { return new PropertyComparison<T>(_propertyName, _contract); }
        }
    }
}
