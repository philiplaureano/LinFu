using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;
using LinFu.Reflection;

namespace UsingClosuresWithDynamicObjects
{
    public class MathClass
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            MathClass math = new MathClass();
            DynamicObject dynamic = new DynamicObject(math);

            // Let the DynamicObject decide which method to call, and
            // give it a set of arguments to use
            Closure closure = new Closure(dynamic.Methods["Add"], 1, 1);

            int result = (int)closure.Invoke();
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
