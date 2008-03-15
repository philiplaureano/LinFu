namespace LinFu.DynamicProxy
{
    public interface IInterceptor
    {
        object Intercept(InvocationInfo info);
    }
}