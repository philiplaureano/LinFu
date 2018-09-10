using System;
using System.Collections.Generic;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using Xunit;
using SampleLibrary;

namespace LinFu.UnitTests.IOC
{
    public partial class FluentExtensionTests
    {
        private static void TestOncePerThread(string serviceName,
            Func<IUsingLambda<ISampleService>, IGenerateFactory<ISampleService>>
                doInject)
        {
            Test(serviceName, factory => factory.OncePerThread(), doInject, VerifyOncePerThread);
        }

        private static void TestSingleton(string serviceName,
            Func<IUsingLambda<ISampleService>, IGenerateFactory<ISampleService>> doInject)
        {
            Test(serviceName, factory => factory.AsSingleton(),
                doInject, VerifySingleton);
        }

        private static void TestOncePerRequest(string serviceName,
            Func<IUsingLambda<ISampleService>, IGenerateFactory<ISampleService>>
                doInject)
        {
            Test(serviceName, factory => factory.OncePerRequest(),
                doInject, VerifyOncePerRequest);
        }

        private static bool VerifySingleton(string serviceName, IServiceContainer container)
        {
            // The container must be able to create the
            // ISampleService instance
            Assert.True(container.Contains(serviceName, typeof(ISampleService)));

            // The container should return the singleton
            var first = container.GetService<ISampleService>(serviceName);
            var second = container.GetService<ISampleService>(serviceName);
            Assert.Same(first, second);

            return true;
        }

        private static bool VerifyOncePerThread(string serviceName, IServiceContainer container)
        {
            var results = new List<ISampleService>();
            Func<ISampleService> createService = () =>
            {
                var result = container.GetService<ISampleService>(serviceName);
                lock (results)
                {
                    results.Add(result);
                }

                return null;
            };

            Assert.True(container.Contains(serviceName, typeof(ISampleService)));

            // Create the other instance from another thread
            var asyncResult = createService.BeginInvoke(null, null);

            // Two instances created within the same thread must be
            // the same
            var first = container.GetService<ISampleService>(serviceName);
            var second = container.GetService<ISampleService>(serviceName);

            Assert.NotNull(first);
            Assert.Same(first, second);

            // Wait for the other thread to finish executing
            createService.EndInvoke(asyncResult);
            Assert.True(results.Count > 0);

            // The service instance created in the other thread
            // must be unique
            Assert.NotNull(results[0]);
            Assert.NotSame(first, results[0]);

            // NOTE: The return value will be ignored
            return true;
        }

        private static bool VerifyOncePerRequest(string serviceName, IServiceContainer container)
        {
            // The container must be able to create an
            // ISampleService instance
            Assert.True(container.Contains(serviceName, typeof(ISampleService)), "Service not found!");

            // Both instances must be unique
            var first = container.GetService<ISampleService>(serviceName);
            var second = container.GetService<ISampleService>(serviceName);
            Assert.NotSame(first, second);

            return true;
        }

        private static void Inject(string serviceName, Action<IGenerateFactory<ISampleService>> usingFactory,
            Func<IUsingLambda<ISampleService>, IGenerateFactory<ISampleService>> doInject,
            ServiceContainer container)
        {
            // HACK: Condense the fluent statements into a single,
            // reusable line of code
            usingFactory(doInject(container.Inject<ISampleService>(serviceName)));
        }

        private static void Test(string serviceName, Action<IGenerateFactory<ISampleService>> usingFactory,
            Func<IUsingLambda<ISampleService>, IGenerateFactory<ISampleService>> doInject,
            Func<string, IServiceContainer, bool> verifyResult)
        {
            var container = new ServiceContainer();

            // HACK: Manually inject the required services into the container
            container.AddDefaultServices();

            Inject(serviceName, usingFactory, doInject, container);
            verifyResult(serviceName, container);
        }
    }
}