using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Delegates
{
    public static class Args
    {
        private static readonly Lambda _lambda = new Lambda();
        public static Lambda Lambda
        {
            get { return _lambda;  }
        }
    }
}
