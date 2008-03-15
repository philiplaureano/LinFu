using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public class PreconditionViolationException : DesignByContractException
    {
        public PreconditionViolationException(string message, InvocationInfo info)
            :
                base(message, info)
        {
        }
    }
}
