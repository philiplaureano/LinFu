using System;

namespace SampleLibrary
{
    public class SampleClass : ISampleService, ISampleGenericService<int>
    {
        #region ISampleGenericService<int> Members

        public bool Called { get; set; }

        #endregion

        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}