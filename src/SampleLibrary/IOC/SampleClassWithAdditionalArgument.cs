namespace SampleLibrary.IOC
{
    public class SampleClassWithAdditionalArgument
    {
        // This is just a dummy constructor used to confuse
        // the resolver
        public SampleClassWithAdditionalArgument(ISampleService arg1)
        {
        }

        public SampleClassWithAdditionalArgument(ISampleService arg1, int arg2)
        {
            Argument = arg2;
        }

        public int Argument { get; }
    }
}