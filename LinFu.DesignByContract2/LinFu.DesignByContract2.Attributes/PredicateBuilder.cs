using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.DesignByContract2.Attributes
{
    //public class PredicateBuilder
    //{
    //    private bool _matchCovariantReturnType;
    //    private bool? _isPublic;
    //    private bool? _isProtected;
    //    private string _methodName;
    //    private Type _returnType;
    //    private List<Type> _typeArguments = new List<Type>();
    //    private List<ParameterInfo> _parameterTypes = new List<ParameterInfo>();
    //    private List<object> _arguments = new List<object>();

    //    public List<object> RuntimeArguments
    //    {
    //        get { return _arguments; }
    //    }
    //    public string MethodName
    //    {            
    //        get { return _methodName;  }
    //        set { _methodName = value; }
    //    }

    //    public Type ReturnType
    //    {
    //        get { return _returnType;  }
    //        set { _returnType = value; }            
    //    }

    //    public List<ParameterInfo> ParameterTypes
    //    {
    //        get { return _parameterTypes; }
    //    }

    //    public List<Type> TypeArguments
    //    {
    //        get { return _typeArguments; }
    //    }

    //    public bool? IsPublic
    //    {
    //        get { return _isPublic; }
    //        set { _isPublic = value; }
    //    }

    //    public bool? IsProtected
    //    {
    //        get { return _isProtected; }
    //        set { _isProtected = value; }
    //    }

    //    public bool MatchCovariantReturnType
    //    {
    //        get { return _matchCovariantReturnType; }
    //        set { _matchCovariantReturnType = value; }
    //    }

    //    public Predicate<MethodInfo> CreatePredicate()
    //    {            
    //        Predicate<MethodInfo> result = null;

    //        #region Match the method name
    //        if (!string.IsNullOrEmpty(_methodName))
    //        {
    //            result += delegate(MethodInfo method)
    //                          {
    //                              return method.Name == _methodName;
    //                          };
    //        }
    //        #endregion

    //        #region Match the return type
    //        if (_returnType != null)
    //        {
    //            result += delegate(MethodInfo method)
    //                          {
    //                              return method.ReturnType == _returnType;
    //                          };
                
    //            if (_matchCovariantReturnType)
    //            {
    //                result += delegate(MethodInfo method)
    //                          {
    //                              return method.ReturnType.IsAssignableFrom(_returnType);
    //                          };
    //            }
    //        }
    //        #endregion

            
    //        #region Match the parameters
    //        if (_parameterTypes.Count > 0)
    //        {
    //            Predicate<MethodInfo> hasParameterTypes = null;
    //            ParameterInfo[] currentParameters = _parameterTypes.ToArray();

    //            // Match the parameter count
    //            int parameterCount = currentParameters != null ? currentParameters.Length : 0;

    //            result += delegate(MethodInfo method)
    //                          {
    //                              ParameterInfo[] parameters = method.GetParameters();
    //                              int count = parameters == null ? 0 : parameters.Length;

    //                              return parameterCount == count;
    //                          };

    //            // Match the parameter types
    //            foreach (ParameterInfo param in currentParameters)
    //            {
    //                int position = param.Position;
    //                Type parameterType = param.ParameterType;
    //                Predicate<MethodInfo> hasParameter = MakeParameterPredicate(position, parameterType);

    //                hasParameterTypes += hasParameter;
    //            }

    //            if (hasParameterTypes != null)
    //                result += hasParameterTypes;
    //        }
    //        #endregion

    //        #region Match the generic type parameters
    //        if (_typeArguments.Count > 0)
    //        {
    //            Predicate<MethodInfo> matchesTypeParameters = null;
    //            int position = 0;
    //            foreach(Type currentType in _typeArguments)
    //            {
    //                matchesTypeParameters += delegate(MethodInfo method)
    //                                             {
    //                                                 if (!method.IsGenericMethod )
    //                                                     return false;
                                                                                                          
    //                                                 Type[] typeArgs = method.GetGenericArguments();
    //                                                 bool isMatch = false;
                                                     
    //                                                 try
    //                                                 {
    //                                                     isMatch = typeArgs[position++] == currentType;
    //                                                 }
    //                                                 catch
    //                                                 {
    //                                                     // Ignore the error 
    //                                                 }
    //                                                 return isMatch; 
    //                                             };                    
    //            }

    //            result += matchesTypeParameters;
    //        }
    //        #endregion

    //        #region Match public methods
    //        if (_isPublic != null && _isPublic.HasValue)
    //        {
    //            result += delegate(MethodInfo method)
    //                          {
    //                              return method.IsPublic == _isPublic;
    //                          };
    //        }
    //        #endregion

    //        #region Match protected methods
    //        if (_isProtected != null && _isProtected.HasValue)
    //        {
    //            result += delegate(MethodInfo method)
    //                          {
    //                              return method.IsFamily == _isProtected;
    //                          };
    //        }
    //        #endregion

    //        #region Match the runtime arguments
    //        if (_arguments.Count > 0)
    //        {
    //            int position = 0;
    //            foreach(object argument in _arguments)
    //            {
    //                if (argument != null)
    //                {
    //                    Type argumentType = argument.GetType();
    //                    result += MakeParameterPredicate(position, argumentType);
    //                }
    //                position++;
    //            }
    //        }
    //        #endregion
    //        return result;
    //    }

    //    private static Predicate<MethodInfo> 
    //        MakeParameterPredicate(int position, Type parameterType)
    //    {
    //        return delegate(MethodInfo method)
    //                   {
    //                       ParameterInfo[] parameters = method.GetParameters();

    //                       bool checkResult = false;
    //                       try
    //                       {
    //                           checkResult = parameters[position].ParameterType == parameterType;
    //                       }
    //                       catch
    //                       {
    //                           // Ignore any errors that occur
    //                       }
    //                       return checkResult;
    //                   };
    //    }

    //    public void SetParameterTypes(ParameterInfo[] parameterInfo)
    //    {
    //        if (parameterInfo != null && parameterInfo.Length > 0)
    //            _parameterTypes.AddRange(parameterInfo);
    //    }
    //}
}
