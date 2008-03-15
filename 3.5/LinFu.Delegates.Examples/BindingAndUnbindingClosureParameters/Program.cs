using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;

namespace BindingAndUnbindingClosureParameters
{
    class Program
    {
        static void Main()
        {
            CustomDelegate addMethodBody = delegate(object[] args)
                         {
                             int a = (int)args[0];
                             int b = (int)args[1];

                             return a + b;
                         };

            // Define the method signature            
            Type returnType = typeof(int);
            Type[] parameterTypes = new Type[] { typeof(int), typeof(int) };

            // Notice that the original parameters use 1 + 1
            Closure closure = new Closure(addMethodBody, returnType, parameterTypes, 1, 1);

            // Change the value of the second parameter to ‘3’
            closure.Arguments[1] = 3;

            // This will return ‘4’ instead of ‘2’
            int result = (int)closure.Invoke();

            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
