namespace LinFu.DynamicProxy
{
    public interface IProxy
    {
        IInterceptor Interceptor { get; set; }
    }
}