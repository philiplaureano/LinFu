using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;

namespace LinFu.DesignByContract2.Tests
{
    public class TestContractProvider : IContractProvider
    {
        private static int _typeContractCallCount;
        private static int _methodContractCallCount;

        public static int MethodContractCallCount
        {
            get { return _methodContractCallCount; }
        }

        public static int TypeContractCallCount
        {
            get { return _typeContractCallCount; }
        }

        #region IContractProvider Members

        public ITypeContract GetTypeContract(Type targetType)
        {
            _typeContractCallCount++;
            return new TypeContract();
        }

        public IMethodContract GetMethodContract(Type targetType, LinFu.DynamicProxy.InvocationInfo info)
        {
            _methodContractCallCount++;
            return new MethodContract();
        }

        #endregion

        public static void ResetTypeContractCallCount()
        {
            _typeContractCallCount = 0;
        }

        public static void ResetMethodContractCallCount()
        {
            _methodContractCallCount = 0;
        }
    }
}
