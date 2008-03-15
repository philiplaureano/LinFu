namespace LinFu.DynamicProxy
{
    public class CallAdapter : IInterceptor
    {
        private IInvokeWrapper _wrapper;

        public CallAdapter(IInvokeWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        #region IInterceptor Members

        public object Intercept(InvocationInfo info)
        {
            object result = null;
            _wrapper.BeforeInvoke(info);
            result = _wrapper.DoInvoke(info);
            _wrapper.AfterInvoke(info, result);

            return result;
        }

        #endregion
    }
}