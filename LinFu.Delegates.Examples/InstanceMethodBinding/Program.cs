using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LinFu.Delegates;
namespace InstanceMethodBinding
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
            MethodInfo addMethod = typeof(MathClass).GetMethod("Add");
            Closure closure = new Closure(math, addMethod, 1, 1);

            int result = (int)closure.Invoke();
            Console.WriteLine("The result is {0}", result);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
