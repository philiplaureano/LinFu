using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Delegates.Tests
{
    class EventSource
    {
        public event EventHandler Event1;
        public void Fire()
        {
            if (Event1 != null)
                Event1(this, new EventArgs());
        }
    }
}
