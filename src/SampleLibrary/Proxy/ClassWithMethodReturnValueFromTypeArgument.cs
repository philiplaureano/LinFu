namespace SampleLibrary.Proxy
{
    public class ClassWithMethodReturnValueFromTypeArgument<T>
    {
        public virtual T DoSomething()
        {
            return default(T);
        }
    }
}