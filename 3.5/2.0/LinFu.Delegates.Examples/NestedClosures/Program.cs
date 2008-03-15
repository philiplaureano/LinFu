using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;
namespace NestedClosures
{
    public delegate int MathOperation(int a, int b);
    class Program
    {
        static void Main(string[] args)
        {
            MathOperation add = delegate(int a, int b) { return a + b; };

            Closure inner = new Closure(add, 1, 1);

            // Evaluate the inner closure and pass its
            // result to the outer closure
            Closure outer = new Closure(add, inner, 1);

            // This will return ‘3’ ( 1 + 1 + 1)
            int result = (int)outer.Invoke();
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
