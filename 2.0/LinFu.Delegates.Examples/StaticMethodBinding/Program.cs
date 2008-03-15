using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LinFu.Delegates;
namespace StaticMethodBinding
{
    class Program
    {
        static void Main(string[] args)
        {
            MethodInfo addMethod = typeof(Program).GetMethod("Add", 
                BindingFlags.Public | BindingFlags.Static);

            Closure closure = new Closure(addMethod, 1, 1);
            int result = (int)closure.Invoke();
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }
}
