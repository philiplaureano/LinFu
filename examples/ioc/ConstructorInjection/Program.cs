using System;
using System.Collections.Generic;
using System.Text;
using CarLibrary3;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace ConstructorInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            IServiceContainer container = new ServiceContainer();

            // Load CarLibrary3.dll; If you need load
            // all the libaries in a directory, use "*.dll" instead
            container.LoadFrom(directory, "CarLibrary3.dll");

            // Configure the container inject instances
            // into the Car class constructor
            container.Inject<IVehicle>()
                .Using(ioc => new Car(ioc.GetService<IEngine>(),
                                      ioc.GetService<IPerson>()))
                                      .OncePerRequest();

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
