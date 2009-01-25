using System.Collections.Generic;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that initializes service instances that use
    /// the <see cref="IInitialize"/> interface.
    /// </summary>
    public class Initializer : Initializer<IServiceContainer>
    {
        /// <summary>
        /// Initializes the class with the default settings.
        /// </summary>
        public Initializer()
            : base(request => request.Container)
        {
        }
    }
}