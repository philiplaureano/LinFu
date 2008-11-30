using System;
using System.Collections.Generic;
using System.Text;
using CarLibrary2;

using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace PropertyInjectionSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            IServiceContainer container = new ServiceContainer();
            

            // Load CarLibrary2.dll; If you need load
            // all the libaries in a directory, use "*.dll" instead
            container.LoadFrom(directory, "CarLibrary2.dll");

            Person person = new Person();
            person.Name = "Someone";
            person.Age = 18;

            container.AddService<IPerson>(person);
            IVehicle vehicle = container.GetService<IVehicle>();
            
            vehicle.Move();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
