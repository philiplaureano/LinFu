namespace SampleLibrary.AOP
{
    public class SampleClassWithMethodHavingOutParameters
    {
        public void DoSomething(out int a)
        {
            a = 12345;
        }
    }
}