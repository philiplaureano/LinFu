using System;
using System.Collections.Generic;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace CarLibrary
{
    [Implements(typeof(IVehicle), LifecycleType.OncePerRequest)]
    public class Car : IVehicle
    {
        private IEngine _engine;
        private IPerson _person;

        // Note: The loader can only work with default constructors
        public Car()
        {
        }
        public IEngine Engine
        {
            get { return _engine; }
            set { _engine = value; }
        }
        public IPerson Driver
        {
            get { return _person; }
            set { _person = value; }
        }
        public void Move()
        {
            if (_engine == null || _person == null)
                return;

            _engine.Start();
            Console.WriteLine("{0} says: I’m moving!", _person.Name);
        }
        public void Park()
        {
            if (_engine == null || _person == null)
                return;

            _engine.Stop();
            Console.WriteLine("{0} says: I’m parked!", _person.Name);
        }
    }

}
