using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// A decorator around an IInitializer object that ensures that
    /// the InitializeType() and Initialize() methods are called only once.
    /// </summary>
    internal class TypeInitializerDecorator : IInitializer
    {
        private List<Type> _initializedTypes = new List<Type>();
        private HashSet<int> _hashes = new HashSet<int>();
        private IInitializer _initializer;
        public TypeInitializerDecorator(IInitializer initializer)
        {
            _initializer = initializer;
        }

        #region IInitializer Members

        public bool CanInitialize(Type targetType)
        {
            return _initializer.CanInitialize(targetType);
        }

        public void CatchError(Exception ex)
        {
            _initializer.CatchError(ex);
        }

        public void Initialize(object target)
        {
            if (target == null)
                return;

            // Initialize an object only once
            if (_hashes.Contains(target.GetHashCode()))
                return;


            Type targetType = target.GetType();
            lock (_initializedTypes)
            {
                if (!_initializedTypes.Contains(targetType))
                {
                    _initializer.InitializeType(targetType);
                    _initializedTypes.Add(targetType);
                }
            }
            _initializer.Initialize(target);

            lock (_hashes)
            {
                _hashes.Add(target.GetHashCode());
            }
        }

        public void InitializeType(Type targetType)
        {
            _initializer.InitializeType(targetType);
        }

        #endregion

        #region IInitializer Members

        public void InitializeSelf()
        {
            _initializer.InitializeSelf();
        }

        #endregion
    }
}
