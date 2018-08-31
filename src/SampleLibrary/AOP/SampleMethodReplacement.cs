using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleMethodReplacement : IInterceptor
    {
        public bool HasBeenCalled { get; private set; }


        public object Intercept(IInvocationInfo info)
        {
            HasBeenCalled = true;
            return null;
        }
    }
}