namespace LinFu.DynamicProxy
{
    public class MethodBody : Interceptor
    {
        private InvocationHandler _handler;

        public MethodBody(InvocationHandler handler)
            : base()
        {
            _handler = handler;
        }

        public override object Intercept(InvocationInfo info)
        {
            return _handler(info);
        }
    }
}