using System;

namespace SampleLibrary.AOP
{
    public class SampleServiceImplementation : ISampleService
    {
        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}