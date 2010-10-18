namespace SampleLibrary.Proxy
{
    public class ClassWithMethodReturnTypeFromGenericTypeArguments
    {
        public virtual T DoSomething<T>()
        {
            return default(T);
        }
    }
}