using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.MxClone.Extensions;

using Simple.IoC;
using Simple.IoC.Extensions;
using Simple.IoC.Loaders;


namespace LinFu.MxClone
{
    public class CompositeTypeResolver : ITypeResolver
    {
        private IList<ITypeResolver> _resolvers = new List<ITypeResolver>();
        public IList<ITypeResolver> Resolvers
        {
            get { return _resolvers; }
        }

        #region ITypeResolver Members

        public Type Resolve(string typename, string assemblyQualifiedName)
        {
            var resolvedTypes = from r in _resolvers
                                let resolvedType = r.Resolve(typename, assemblyQualifiedName)
                                where resolvedType != null
                                select resolvedType;

            var resolvedTypeList = resolvedTypes.ToList();


            if (resolvedTypeList.Count == 0)
                return null;

            return resolvedTypeList.First();
        }

        #endregion
    }
}
