namespace SampleLibrary.Proxy
{
    public class SampleClassWithPropertyInitializedInCtor
    {
        public SampleClassWithPropertyInitializedInCtor()
        {
            SomeProp = "abcdefg";
        }

        public virtual string SomeProp { get; set; }

        public virtual void DoSomething(out int value)
        {
            value = 54321;
        }
    }
}