using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IServiceInstance"/> interface.
    /// </summary>
    internal class ServiceInstance : IServiceInstance
    {
        #region IServiceInstance Members

        public IServiceInfo ServiceInfo { get; internal set; }

        public object Object { get; internal set; }

        #endregion
    }
}