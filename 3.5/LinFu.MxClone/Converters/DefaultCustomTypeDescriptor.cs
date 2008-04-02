using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using Simple.IoC;
using Simple.IoC.Loaders;
using Simple.IoC.Extensions;
using System.IO;

namespace LinFu.MxClone
{
    [Implements(typeof(ICustomTypeDescriptor), LifecycleType.OncePerRequest)]
    public class DefaultCustomTypeDescriptor : ICustomTypeDescriptor, IInitialize
    {
        private IList<ICustomTypeConverter> _converters = new List<ICustomTypeConverter>();

        public IList<ICustomTypeConverter> TypeConverters
        {
            get { return _converters; }
        }
        #region ICustomTypeDescriptor Members

        public ICustomTypeConverter GetConverter(Type targetType)
        {
            var matches = (from c in _converters
                          where c.CanConvertTo(targetType)
                          select c).ToList();

            if (matches.Count == 0)
                return null;

            return matches.First();
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            // Load all of the converters from the library directory
            string directory = Path.GetDirectoryName(typeof(ICustomTypeConverter).Assembly.Location);
            _converters.CollectWith(container, directory, "*.dll");
        }

        #endregion
    }
}
