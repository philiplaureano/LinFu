namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a class that can inspect or modify service requests
    /// from a given container once a service is created.
    /// </summary>
    public interface IPostProcessor
    {
        /// <summary>
        /// Allows a <see cref="IPostProcessor"/> instance
        /// to inspect or modify the result of a service request.
        /// </summary>
        /// <seealso cref="IServiceRequestResult"/>
        /// <param name="result">The <see cref="IServiceRequestResult"/> created as a result of the container operation.</param>
        void PostProcess(IServiceRequestResult result);
    }
}