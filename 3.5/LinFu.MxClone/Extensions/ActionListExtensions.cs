using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;

using LinFu.Reflection.Extensions;
namespace LinFu.MxClone.Extensions
{
    public static class ActionListExtensions
    {
        public static void ExecuteAll(this IEnumerable<IAction> actions)
        {
            actions.ForEach(a => a.Execute());
        }
    }
}
