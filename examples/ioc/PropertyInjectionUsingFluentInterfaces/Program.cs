using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarLibrary3;
using CarLibrary4;
using LinFu.IoC;
using LinFu.IoC.Interfaces;

namespace PropertyInjectionUsingFluentInterfaces
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

            // Inject the OldPerson type
            container.Inject<IPerson>("OldPerson").Using<Person>();
            container.Initialize<IPerson>("OldPerson")
                .With(p =>
                {
                    p.Age = 99;
                    p.Name = "OldPerson";
                });

            // Inject the YoungPerson type
            container.Inject<IPerson>("YoungPerson").Using<Person>();
            container.Initialize<IPerson>("YoungPerson")
                .With(p =>
                {
                    p.Age = 16;
                    p.Name = "YoungPerson";
                });

            // Inject the DeadEngine type
            container.Inject<IEngine>("DeadEngine").Using<DeadEngine>()
                .OncePerRequest();

            // Inject the OldEngine type
            container.Inject<IEngine>("OldEngine").Using<OldEngine>()
                .OncePerRequest();

            // Inject the BrokenVehicle type into the container
            container.Inject<IVehicle>("BrokenVehicle")
                .Using<Car>().OncePerRequest();

            #region Broken Vehicle Configuration

            AddVehicle(container, "BrokenVehicle", "DeadEngine", "YoungPerson");

            #endregion

            #region Old Vehicle Configuration

            AddVehicle(container, "OldVehicle", "OldEngine", "OldPerson");

            #endregion

            // Inject the OldVehicle type into the container
            container.Inject<IVehicle>("OldVehicle")
                .Using<Car>().OncePerRequest();

            Person person = new Person();
            person.Name = "Someone";
            person.Age = 18;

            container.AddService<IPerson>(person);
            var brokenVehicle = container.GetService<IVehicle>("BrokenVehicle");
            var oldVehicle = container.GetService<IVehicle>("OldVehicle");

            Console.Write("Broken Vehicle: ");
            brokenVehicle.Move();

            Console.Write("Old Vehicle: ");
            oldVehicle.Move();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private static void AddVehicle(IServiceContainer container, string vehicleName, string engineName, string personName)
        {
            container.Initialize<IVehicle>(vehicleName)
                .With((ioc, vehicle) => vehicle.Engine = ioc.GetService<IEngine>(engineName));

            // Set the person type
            container.Initialize<IVehicle>(vehicleName)
                .With((ioc, vehicle) => vehicle.Driver = ioc.GetService<IPerson>(personName));
        }
    }
}
