using System.Collections.Generic;

namespace LinFu.DesignByContract2.Core
{
    public interface ITypeContract
    {
        IList<IInvariant> Invariants { get; }
    }
}