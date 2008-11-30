namespace LinFu.Reflection
{
    /// <summary>
    /// Represents classes that need to be initialized
    /// every time a particular 
    /// instance creates that type.
    /// </summary>
    public interface IInitialize<T>
    {
        /// <summary>
        /// Initializes the target with the
        /// particular <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="source">The <typeparamref name="T"/> instance that will hold the target type.</param>
        void Initialize(T source);
    }
}