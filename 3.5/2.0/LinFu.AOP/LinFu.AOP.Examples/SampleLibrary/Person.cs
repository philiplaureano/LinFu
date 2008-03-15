using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary
{
    public sealed class Person
    {
        public void Speak()
        {
            Console.WriteLine("Hello, World!");
        }
        public static void SayHello(string name)
        {
            Console.WriteLine("Hello, {0}!", name);
        }
    }
}
