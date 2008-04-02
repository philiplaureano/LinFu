using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface IActionBuilder
    {
        void Build(IParserContext context, IList<IAction> results);
    }
}
