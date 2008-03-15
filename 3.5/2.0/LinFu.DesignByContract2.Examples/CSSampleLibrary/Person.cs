using System;
using System.Collections.Generic;
using System.Text;
using CSContractAssertions;
using LibraryInterfaces;

namespace CSSampleLibrary
{
    public class Person : IPerson
    {
        private int _age;
        private string _name;

        public Person(string name, int age)
        {
            _age = age;
            _name = name;
        }
        public virtual string Name
        {
           get { return _name;  }   
           set { _name = value; }
        }
        public virtual int Age 
        {
            [return: NonNegative] get { return _age;  }
            set { _age = value; }
        }

        #region IPerson Members

        string IPerson.Name
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        int IPerson.Age
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
