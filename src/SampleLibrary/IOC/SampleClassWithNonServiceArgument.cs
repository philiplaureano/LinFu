namespace SampleLibrary.IOC
{
    public class SampleClassWithNonServiceArgument
    {
        public SampleClassWithNonServiceArgument(string value)
        {
            Value = value;
        }

        // This is just a dummy constructor that
        // will be used to confuse the constructor resolver
        public SampleClassWithNonServiceArgument(string arg1, string arg2)
        {
        }

        // This is just another dummy constructor that
        // will be used to confuse the constructor resolver
        public SampleClassWithNonServiceArgument(string arg1, string arg2, string arg3)
        {
        }

        public string Value { get; }
    }
}