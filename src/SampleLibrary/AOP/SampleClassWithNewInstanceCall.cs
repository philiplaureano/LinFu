namespace SampleLibrary.AOP
{
    public class SampleClassWithNewInstanceCall
    {
        public ISampleService DoSomething()
        {
            return new SampleServiceImplementation();
        }
    }
}