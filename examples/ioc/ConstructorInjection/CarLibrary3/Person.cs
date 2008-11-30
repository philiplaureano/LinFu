using System;
using System.Collections.Generic;
using System.Text;

using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace CarLibrary3
{
    [Implements(typeof(IPerson), LifecycleType.OncePerRequest)]
    public class Person : IPerson
    {
        private string _name;
        private int _age;
        #region IPerson Members

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
            }
        }

        #endregion
    }
}
