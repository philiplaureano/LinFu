using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleInterceptor : IInterceptor
    {
        public bool HasBeenInvoked { get; set; }


        public object Intercept(IInvocationInfo info)
        {
            HasBeenInvoked = true;
            return null;
        }
    }
}