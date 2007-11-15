using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;
namespace ClosuresAndDynamicallyGeneratedMethods
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
            Closure closure = new Closure(addMethodBody, returnType, parameterTypes, 1, 1);

            int result = (int)closure.Invoke();
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
