using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public class PostconditionViolationException : DesignByContractException
    {
        public PostconditionViolationException(string message, InvocationInfo info)
            :
                base(message, info)
        {
        }
    }
}