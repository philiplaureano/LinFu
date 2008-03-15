using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;

namespace DelegatesAndArgumentBinding
{
    public delegate int MathOperation(int a, int b);
    class Program
    {
        static void Main()
        {
            MathOperation add = delegate(int first, int second) { return first + second; };

            // Derive a new add method and 
            // set the second parameter value = 1
            Closure addOne = new Closure(add, Args.Lambda, 1);

            // This will return ‘3’
            int result = (int)addOne.Invoke(2);
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
