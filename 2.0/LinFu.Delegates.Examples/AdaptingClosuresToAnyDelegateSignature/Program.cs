using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;
namespace AdaptingClosuresToAnyDelegateSignature
{
    public delegate int MathOperation(int a, int b);
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

            // Specify the method signature
            Type returnType = typeof(int);
            Type[] parameterTypes = new Type[] { typeof(int), typeof(int) };

            // Leave both arguments open
            Closure closure = new Closure(addMethodBody, returnType, parameterTypes,
                Args.Lambda, Args.Lambda);

            // Make the closure appear to be a MathOperation delegate
            MathOperation add = closure.AdaptTo<MathOperation>();
            int result = add(3, 3);
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
