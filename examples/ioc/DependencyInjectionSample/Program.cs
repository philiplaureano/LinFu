using System;
using System.Collections.Generic;
using System.Text;
using CarLibrary;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace DependencyInjectionSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            var container = new ServiceContainer();

            // Load CarLibrary.dll; If you need load
            // all the libaries in a directory, use "*.dll" instead
            container.LoadFrom(directory, "CarLibrary.dll");

            IVehicle vehicle = container.GetService<IVehicle>();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            return;
        }
    }
}
