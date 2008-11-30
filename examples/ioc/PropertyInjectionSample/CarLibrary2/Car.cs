using System;
using System.Collections.Generic;
using System.Text;
using CarLibrary2;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;


namespace CarLibrary2
{
    [Implements(typeof(IVehicle), LifecycleType.OncePerRequest)]
    public class Car : IVehicle, IInitialize
    {
        private IEngine _engine;
        private IPerson _person;

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

        #region IInitialize Members

        public void Initialize(IServiceContainer container)
        {
            _engine = container.GetService<IEngine>();
            _person = container.GetService<IPerson>();
        }

        #endregion
    }

}
