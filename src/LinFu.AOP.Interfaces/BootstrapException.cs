using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    [Serializable]
    public class BootstrapException : Exception
    {
        public BootstrapException(string message, Exception ex) : base(message, ex)
        {                        
        }
    }
}
