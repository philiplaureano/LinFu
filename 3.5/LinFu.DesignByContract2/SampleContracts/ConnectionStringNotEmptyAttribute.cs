using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConnectionStringNotEmptyAttribute : Attribute, IPrecondition
    {
        #region IPrecondition Members

        public bool Check(object target, InvocationInfo info)
        {
            IDbConnection connection = target as IDbConnection;
            if (connection == null)
                return true;

            return !string.IsNullOrEmpty(connection.ConnectionString);
        }

        public void ShowError(System.IO.TextWriter output, object target, InvocationInfo info)
        {
            output.WriteLine("The connection string cannot be null or empty!");
        }

        #endregion

        #region IMethodContractCheck Members

        public bool AppliesTo(object target, LinFu.DynamicProxy.InvocationInfo info)
        {
            IDbConnection connection = target as IDbConnection;
            if (connection == null)
                return false;
            
            return true;
        }

        public void Catch(Exception ex)
        {

        }
        #endregion
    }
}
