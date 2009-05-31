using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.AOP
{
    public class SampleClassWithReadOnlyField
    {
        private string fieldValue = "freeze!";

        public string Value
        {
            get { return fieldValue; }
            set { fieldValue = value; }
        }
    }
}
