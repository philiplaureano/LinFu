using System;
using System.Collections.Generic;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace Extensibility
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            IServiceContainer container = new ServiceContainer();

            container.LoadFrom(directory, "*.dll");

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
