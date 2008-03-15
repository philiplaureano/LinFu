using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public class InvariantViolationException : DesignByContractException
    {
        public InvariantViolationException(string message, InvocationInfo info)
            :
                base(message, info)
        {
        }
    }
}
