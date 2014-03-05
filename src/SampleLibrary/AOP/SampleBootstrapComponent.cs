using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleBootstrapComponent : IBootStrappedComponent
    {
        public bool Called { get; private set; }


        public void Initialize()
        {
            Called = true;
        }
    }
}