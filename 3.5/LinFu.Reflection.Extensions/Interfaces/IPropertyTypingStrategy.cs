using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public interface IPropertyTypingStrategy
    {
        bool PropertyMatches(string propertyName, Type propertyType);
    }
}
