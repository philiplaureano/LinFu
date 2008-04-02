using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    [Serializable]
    public class MethodSpec
    {
        private List<string> _aliases = new List<string>();
        private List<Type> _parameterTypes = new List<Type>();

        public MethodSpec()
        {
            ReturnType = typeof(void);
        }
        public string MethodName { get; set; }
        public Type ReturnType { get; set; }
        public List<Type> ParameterTypes
        {
            get { return _parameterTypes; }
        }
        public List<string> Aliases
        {
            get { return _aliases; }
        }
        public IMethodBody MethodBody
        {
            get;
            set;
        }
    }
}
