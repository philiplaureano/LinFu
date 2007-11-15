using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.Common;
using LinFu.Delegates;

namespace LinFu.Reflection
{
    internal class Binder : IObjectMethods, IObjectProperties
    {
        private object _target;
        private readonly IMethodFinder _finder;
        private DynamicObject _dynamicObject;
        public Binder(object target, IMethodFinder finder, DynamicObject dynamicObject)
        {
            _target = target;
            _finder = finder;
            _dynamicObject = dynamicObject;
        }
        #region IObjectMethods Members

        public CustomDelegate this[string methodName]
        {
            get
            {
                CustomDelegate result = delegate(object[] args)
                                             {
                                                 if (_target == null)
                                                     throw new NullReferenceException("No target instance found!");

                                                 MethodInfo bestMatch =
                                                     _finder.Find(methodName, _target.GetType(), args);

                                                 object returnValue = null;
                                                 
                                                 if (bestMatch == null)
                                                 {
                                                     bool handled = false;
                                                     returnValue = _dynamicObject.ExecuteMethodMissing(methodName, args, ref handled);                                                     
                                                     if (handled)
                                                         return returnValue;

                                                     throw new NotImplementedException();
                                                 }

                                                 try
                                                 {
                                                     returnValue = bestMatch.Invoke(_target, args);
                                                 }
                                                 catch (TargetInvocationException ex)
                                                 {
                                                     throw ex.InnerException;
                                                 }

                                                 return returnValue;
                                             };
                return result;
            }
        }

        #endregion



        #region IObjectProperties Members

        object IObjectProperties.this[string propertyName]
        {
            get
            {
                string methodName = string.Format("get_{0}", propertyName);
                IObjectMethods methods = this;
                return methods[methodName]();
            }
            set
            {
                string methodName = string.Format("set_{0}", propertyName);
                IObjectMethods methods = this;
                methods[methodName](value);
            }
        }

        #endregion
    }
}
