using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Delegates
{
    internal struct DelegateInfo
    {
        public DelegateInfo(Type returnType, Type[] parameterTypes)
        {
            ReturnType = returnType;
            Parameters = parameterTypes;
        }
        public Type ReturnType;
        public Type[] Parameters;
        public override bool Equals(object obj)
        {
            if (!(obj is DelegateInfo))
                return false;

            DelegateInfo info = (DelegateInfo)obj;

            return Compare(this, info);
        }
  
        private static bool Compare(DelegateInfo lhs, DelegateInfo rhs)
        {
            return lhs.ReturnType == rhs.ReturnType &&
                CompareParameters(lhs, rhs.Parameters) == true;
        }
        private static bool CompareParameters(DelegateInfo info, Type[] parameterTypes)
        {
            if (info.Parameters == null && parameterTypes == null)
                return true;

            if (info.Parameters.Length != parameterTypes.Length)
                return false;

            for (int position = 0; position < parameterTypes.Length; position++)
            {
                if (info.Parameters[position] != parameterTypes[position])
                    return false;
            }

            return true;
        }

    }
}
