using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    [Serializable]
    public class PropertySpec
    {
        private List<string> _aliases = new List<string>();
        public PropertySpec()
        {
            CanRead = true;
            CanWrite = true;
        }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public IPropertyBehavior Behavior { get; set; }
        public List<string> Aliases
        {
            get { return _aliases; }
        }
    }
}
