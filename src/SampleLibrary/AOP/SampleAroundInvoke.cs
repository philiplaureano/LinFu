using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleAroundInvoke : IAroundInvoke
    {
        public bool BeforeInvokeWasCalled { get; private set; }

        public bool AfterInvokeWasCalled { get; private set; }

        #region IAroundInvoke Members

        public void BeforeInvoke(IInvocationInfo info)
        {
            BeforeInvokeWasCalled = true;
        }

        public void AfterInvoke(IInvocationInfo info, object returnValue)
        {
            AfterInvokeWasCalled = true;
        }

        #endregion
    }
}