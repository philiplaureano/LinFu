using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LinFu.DesignByContract2.Tests
{
    public class MethodFinderTargetDummy
    {
        public void DoSomething(int a, int b)
        {
            throw new NotImplementedException();
        }
        public int DoSomethingElse()
        {
            throw new NotImplementedException();
        }
        public T DoSomethingGeneric<T> ()
        {
            throw new NotImplementedException();
        }
        public void OverloadedMethod(string arg1, string arg2)
        {
            throw new NotImplementedException();
        }
        public void OverloadedMethod(object arg1, object arg2)
        {
            throw new NotImplementedException();
        }
        public void OverloadedMethod(int arg1, object arg2)
        {
            throw new NotImplementedException();
        }
        public void OverloadedMethod(int arg1, string arg2)
        {
            throw new NotImplementedException();
        }
        public IDbConnection ReturnConnection()
        {
            throw new NotImplementedException();
        }
    }
}
