namespace LinFu.AOP.Interfaces
{
    internal interface ICallCounter
    {
        void Increment(IInvocationInfo context);
        void Decrement(IInvocationInfo context);
        int GetPendingCalls(IInvocationInfo context);
    }
}