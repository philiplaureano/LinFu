namespace SampleLibrary.IOC
{
    public class SampleLazyService : ISampleService
    {
        public SampleLazyService()
        {
            IsInitialized = true;
        }

        public static bool IsInitialized { get; private set; }

        #region ISampleService Members

        public void DoSomething()
        {
            IsInitialized = true;
        }

        #endregion

        public static void Reset()
        {
            IsInitialized = false;
        }
    }
}