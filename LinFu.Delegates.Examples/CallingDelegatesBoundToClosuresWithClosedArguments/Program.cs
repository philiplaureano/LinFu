using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;
namespace CallingDelegatesBoundToClosuresWithClosedArguments
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

            Type returnType = typeof(int);
            Type[] parameterTypes = new Type[] { typeof(int), typeof(int) };

            // Assign ‘3’ to both parameters
            Closure closure = new Closure(addMethodBody, returnType, parameterTypes,
                                          3, 3);

            MathOperation add = closure.AdaptTo<MathOperation>();

            // This will return ‘6’ (3 + 3)
            int result = add(1, 2);
            Console.WriteLine("The first result is {0}", result);

            // Open the first parameter
            closure.Arguments[0] = Args.Lambda;

            // This will return ‘4’ (3 + 1)
            result = add(1, 2);
            Console.WriteLine("The second result is {0}", result);
            // Open both arguments
            closure.Arguments[1] = Args.Lambda;

            // This will return ‘3’ (1 + 2)
            result = add(1, 2);
            Console.WriteLine("The final result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
