using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Reflection
{
    public class PredicateBuilder
    {
        private bool _matchCovariantReturnType;
        private bool _matchCovariantParameterTypes;
        private bool _matchRuntimeArguments;
        private bool? _isPublic;
        private bool? _isProtected;
        private bool _matchParameterTypes;
        private string _methodName;
        private Type _returnType;
        private readonly List<Type> _argumentTypes = new List<Type>();
        private readonly List<Type> _typeArguments = new List<Type>();
        private readonly List<ParameterInfo> _parameterTypes = new List<ParameterInfo>();
        private readonly List<object> _arguments = new List<object>();
        private bool _matchParameters = true;

        public List<Type> ArgumentTypes
        {
            get { return _argumentTypes; }
        } 
        public List<object> RuntimeArguments
        {
            get { return _arguments; }
        }
        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; }
        }

        public Type ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }

        public List<ParameterInfo> ParameterTypes
        {
            get { return _parameterTypes; }
        }

        public List<Type> TypeArguments
        {
            get { return _typeArguments; }
        }

        public bool? IsPublic
        {
            get { return _isPublic; }
            set { _isPublic = value; }
        }

        public bool? IsProtected
        {
            get { return _isProtected; }
            set { _isProtected = value; }
        }

        public bool MatchCovariantReturnType
        {
            get { return _matchCovariantReturnType; }
            set { _matchCovariantReturnType = value; }
        }

        public bool MatchParameters
        {
            get { return _matchParameters; }
            set { _matchParameters = value; }
        }

        public bool MatchParameterTypes
        {
            get { return _matchParameterTypes; }
            set { _matchParameterTypes = value; }
        }
        public bool MatchCovariantParameterTypes
        {
            get { return _matchCovariantParameterTypes; }
            set { _matchCovariantParameterTypes = value; }
        }

        public bool MatchRuntimeArguments
        {
            get { return _matchRuntimeArguments; }
            set { _matchRuntimeArguments = value; }
        }
        public static Predicate<MethodInfo> CreatePredicate(MethodInfo method)
        {
            PredicateBuilder builder = new PredicateBuilder();
            builder.MatchParameters = true;
            builder.MethodName = method.Name;

            foreach (ParameterInfo param in method.GetParameters())
            {
                builder.ParameterTypes.Add(param);
            }

            // Match any type arguments
            if (method.IsGenericMethod)
            {
                foreach (Type current in method.GetGenericArguments())
                {
                    builder.TypeArguments.Add(current);
                }
            }
            builder.ReturnType = method.ReturnType;

            Predicate<MethodInfo> predicate = builder.CreatePredicate();

            return predicate;
        }
        public Predicate<MethodInfo> CreatePredicate()
        {
            Predicate<MethodInfo> result = null;

            #region Match the method name
            if (!string.IsNullOrEmpty(_methodName))
            {
                Predicate<MethodInfo> shouldMatchMethodName =
                    delegate(MethodInfo method)
                    {
                        return method.Name == _methodName;
                    };

                // Results that match the method name will get a higher
                // score
                result += shouldMatchMethodName;
                result += shouldMatchMethodName;
            }
            #endregion

            #region Match the return type
            if (_returnType != null)
            {
                result += delegate(MethodInfo method)
                              {
                                  return method.ReturnType == _returnType;
                              };

                if (_matchCovariantReturnType)
                {
                    result += delegate(MethodInfo method)
                              {
                                  return method.ReturnType.IsAssignableFrom(_returnType);
                              };
                }
            }
            #endregion


            #region Match the parameters
            if (_matchParameters && _parameterTypes.Count > 0)
            {
                Predicate<MethodInfo> hasParameterTypes = null;
                ParameterInfo[] currentParameters = _parameterTypes.ToArray();

                // Match the parameter count
                int parameterCount = currentParameters != null ? currentParameters.Length : 0;

                result += delegate(MethodInfo method)
                              {
                                  ParameterInfo[] parameters = method.GetParameters();
                                  int count = parameters == null ? 0 : parameters.Length;

                                  return parameterCount == count;
                              };

                // Match the parameter types
                foreach (ParameterInfo param in currentParameters)
                {
                    int position = param.Position;
                    Type parameterType = param.ParameterType;
                    Predicate<MethodInfo> hasParameter = MakeParameterPredicate(position, parameterType, _matchCovariantParameterTypes);

                    hasParameterTypes += hasParameter;
                }

                if (hasParameterTypes != null)
                    result += hasParameterTypes;
            }


            // Check for zero parameters
            if (_matchParameters && _parameterTypes.Count == 0 && !_matchRuntimeArguments)
            {
                result += delegate(MethodInfo currentMethod)
                              {
                                  ParameterInfo[] currentParameters = currentMethod.GetParameters();

                                  // Match the parameter count
                                  int parameterCount = currentParameters != null ? currentParameters.Length : 0;
                                  return parameterCount == 0;
                              };
            }
            #endregion

            #region Match the generic type parameters
            if (_typeArguments.Count > 0)
            {
                Predicate<MethodInfo> matchesTypeParameters = null;
                int position = 0;
                foreach (Type currentType in _typeArguments)
                {
                    matchesTypeParameters += delegate(MethodInfo method)
                                                 {
                                                     if (!method.IsGenericMethod)
                                                         return false;

                                                     Type[] typeArgs = method.GetGenericArguments();
                                                     bool isMatch = false;

                                                     try
                                                     {
                                                         isMatch = typeArgs[position++] == currentType;
                                                     }
                                                     catch
                                                     {
                                                         // Ignore the error 
                                                     }
                                                     return isMatch;
                                                 };
                }

                result += matchesTypeParameters;
            }

            if (_typeArguments.Count == 0)
            {
                result += delegate(MethodInfo currentMethod)
                              {
                                  Type[] currentParameterTypes = currentMethod.GetGenericArguments();

                                  // Match the Type parameter
                                  int parameterCount = currentParameterTypes != null ? currentParameterTypes.Length : 0;
                                  return parameterCount == 0;
                              };
            }
            #endregion

            #region Match public methods
            if (_isPublic != null && _isPublic.HasValue)
            {
                result += delegate(MethodInfo method)
                              {
                                  return method.IsPublic == _isPublic;
                              };
            }
            #endregion

            #region Match protected methods
            if (_isProtected != null && _isProtected.HasValue)
            {
                result += delegate(MethodInfo method)
                              {
                                  return method.IsFamily == _isProtected;
                              };
            }
            #endregion

            #region Match the runtime arguments
            if (_arguments.Count > 0 && MatchRuntimeArguments)
            {
                int position = 0;
                foreach (object argument in _arguments)
                {
                    if (argument != null)
                    {
                        Type argumentType = argument.GetType();
                        result += MakeParameterPredicate(position, argumentType, _matchCovariantParameterTypes);
                    }
                    position++;
                }
            }

            if (_arguments.Count == 0 && MatchRuntimeArguments)
            {
                result += delegate(MethodInfo currentMethod)
                              {
                                  ParameterInfo[] currentParameters = currentMethod.GetParameters();

                                  // Match the parameter count
                                  int parameterCount = currentParameters != null ? currentParameters.Length : 0;
                                  return parameterCount == 0;
                              };
            }
            #endregion

            #region Match the parameter types
            if (MatchParameterTypes && _argumentTypes.Count > 0)
            {
                int position = 0;
                foreach (Type currentType in _argumentTypes)
                {
                    result += MakeParameterPredicate(position, currentType, false);
                    position++;
                }
            }
            #endregion
            return result;
        }

        private static Predicate<MethodInfo>
            MakeParameterPredicate(int position, Type parameterType, bool covariant)
        {

            Predicate<MethodInfo> result = delegate(MethodInfo method)
                       {
                           ParameterInfo[] parameters = method.GetParameters();

                           bool checkResult = false;
                           try
                           {
                               if (!covariant)
                                   checkResult = parameters[position].ParameterType == parameterType;

                               if (covariant)
                                   checkResult = parameters[position].ParameterType.IsAssignableFrom(parameterType);
                           }
                           catch
                           {
                               // Ignore any errors that occur
                           }
                           return checkResult;
                       };

            return result;
        }

        public void SetParameterTypes(ParameterInfo[] parameterInfo)
        {
            if (parameterInfo != null && parameterInfo.Length > 0)
                _parameterTypes.AddRange(parameterInfo);
        }
    }
}
