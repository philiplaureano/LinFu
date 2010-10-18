using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleBootstrapComponent : IBootStrappedComponent
    {
        public bool Called { get; private set; }

        #region IBootStrappedComponent Members

        public void Initialize()
        {
            Called = true;
        }

        #endregion
    }
}