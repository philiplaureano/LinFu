using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public delegate bool PropertyComparisonHandler<T>(T oldValue, T newValue);
}
