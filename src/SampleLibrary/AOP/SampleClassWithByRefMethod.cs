namespace SampleLibrary.AOP
{
    public class SampleClassWithByRefMethod
    {
        public void ByRefMethod(ref object a)
        {
        }

        public void NonByRefMethod(object a)
        {
        }
    }
}