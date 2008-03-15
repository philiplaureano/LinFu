using System.Collections.Generic;

namespace LinFu.DesignByContract2.Core
{
    public class TypeContract : ITypeContract
    {
        private List<IInvariant> _invariants = new List<IInvariant>();
        
        public IList<IInvariant> Invariants
        {
            get { return _invariants; }
        }
    }
}