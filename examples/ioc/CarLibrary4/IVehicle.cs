using System;
using System.Collections.Generic;
using System.Text;

namespace CarLibrary3
{
    public interface IVehicle
    {
        void Move();
        void Park();
        IPerson Driver { get; set; }
        IEngine Engine { get; set; }
    }
}
