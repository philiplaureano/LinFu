namespace SampleLibrary.IOC
{
    public class SampleLazyService : ISampleService
    {
        public SampleLazyService()
        {
            IsInitialized = true;
        }

        public static bool IsInitialized { get; private set; }


        public void DoSomething()
        {
            IsInitialized = true;
        }


        public static void Reset()
        {
            IsInitialized = false;
        }
    }
}