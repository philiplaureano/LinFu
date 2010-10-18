namespace SampleLibrary.Proxy
{
    public class ClassWithVirtualMethodWithOutParameter
    {
        public virtual void DoSomething(out int a)
        {
            a = 12345;
        }
    }
}