using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// Represents a class that uses a <see cref="MulticastDelegate"/>
    /// to instantiate a service instance.
    /// </summary>
    public class DelegateFactory : IFactory
    {
        private readonly MulticastDelegate _targetDelegate;
        /// <summary>
        /// Initializes the class with the given <paramref name="targetDelegate"/>
        /// </summary>
        /// <param name="targetDelegate">The delegate that will be used to instantiate the factory.</param>
        public DelegateFactory(MulticastDelegate targetDelegate)
        {
            if (targetDelegate.Method.ReturnType == typeof(void))
                throw new ArgumentException("The factory delegate must have a return type.");

            _targetDelegate = targetDelegate;
        }

        /// <summary>
        /// Instantiates the service type using the given delegate.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> that describes the service that needs to be created.</param>
        /// <returns>The service instance.</returns>
        public object CreateInstance(IFactoryRequest request)
        {
            object result = null;
            try
            {
                var target = _targetDelegate.Target;
                var method = _targetDelegate.Method;
                var argCount = request.Arguments.Length;
                var methodArgCount = method.GetParameters().Count();

                if (argCount != methodArgCount)
                    Console.WriteLine("Parameter Count Mismatch");
                result = _targetDelegate.DynamicInvoke(request.Arguments);
                //result = method.Invoke(target, request.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                // Unroll the exception
                throw ex.InnerException;
            }

            return result;
        }
    }
}
