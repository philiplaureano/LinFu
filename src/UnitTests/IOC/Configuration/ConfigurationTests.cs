using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;
using Moq;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void AssemblyLoaderMustLoadTargetAssemblyFromDisk()
        {
            IAssemblyLoader loader = new AssemblyLoader();

            // The loader should return a valid assembly
            Assembly result = loader.Load(typeof(SampleClass).Assembly.Location);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ClassMarkedWithPostProcessorAttributeMustBeInjectedIntoContainer()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var loader = new Loader();
            loader.LoadDirectory(baseDirectory, "*.dll");
            var container = new ServiceContainer();

            loader.LoadInto(container);

            IEnumerable<IPostProcessor> matches = from p in container.PostProcessors
                                                  where p != null &&
                                                        p.GetType() == typeof(SamplePostProcessor)
                                                  select p;

            Assert.IsTrue(matches.Count() > 0, "The postprocessor failed to load.");
        }

        [Test]
        public void ClassMarkedWithPreprocessorAttributeMustBeInjectedIntoContainer()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var loader = new Loader();
            loader.LoadDirectory(baseDirectory, "*.dll");
            var container = new ServiceContainer();

            loader.LoadInto(container);

            IEnumerable<IPreProcessor> matches = from p in container.PreProcessors
                                                  where p != null &&
                                                        p.GetType() == typeof(SamplePreprocessor)
                                                  select p;

            Assert.IsTrue(matches.Count() > 0, "The preprocessor failed to load.");
        }
        [Test]
        public void CreatedServicesMustBeAbleToInitializeThemselves()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var loader = new Loader();
            loader.LoadDirectory(baseDirectory, "*.dll");

            var mockInitialize = new Mock<IInitialize>();
            var container = new ServiceContainer();

            // The service container should call the Initialize method
            mockInitialize.Expect(target => target.Initialize(container));
            loader.LoadInto(container);

            container.AddService(mockInitialize.Object);

            var result = container.GetService<IInitialize>();
            Assert.IsNotNull(result);
            Assert.AreSame(mockInitialize.Object, result);

            mockInitialize.VerifyAll();
        }

        [Test]
        public void LoaderMustCallCustomLoaderActions()
        {
            var mockContainer = new Mock<IServiceContainer>();
            var mockListing = new Mock<IDirectoryListing>();

            var loader = new Loader<IServiceContainer>();
            loader.DirectoryLister = mockListing.Object;

            // Return an empty file listing
            mockListing.Expect(listing => listing.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[0]);

            // Use the initializer mock to verify
            // that the custom action was called

            var mockInitializer = new Mock<IInitialize>();
            mockInitializer.Expect(initializer => initializer.Initialize(mockContainer.Object));

            // HACK: In order to verify the call,
            // we need to adapt the mock to an 
            // Action<ILoader<IServiceContainer>> instance
            Action<ILoader<IServiceContainer>> customAction =
                targetLoader =>
                    {
                        IInitialize initializer = mockInitializer.Object;
                        IServiceContainer container = mockContainer.Object;

                        // The test will only succeed if
                        // the following line of code
                        // is invoked:
                        initializer.Initialize(container);
                    };

            loader.CustomLoaderActions.Add(customAction);
            loader.LoadInto(mockContainer.Object);

            mockInitializer.VerifyAll();
        }

        [Test]
        public void LoaderMustPassFilenameToContainerLoaders()
        {
            var mockContainer = new Mock<IServiceContainer>();
            var mockLoader = new Mock<IContainerLoader>(MockBehavior.Loose);
            var mockListing = new Mock<IDirectoryListing>();

            var loader = new Loader<IServiceContainer>()
                             {
                                 DirectoryLister = mockListing.Object
                             };

            string filename = "input.dll";
            mockListing.Expect(listing => listing.GetFiles(It.IsAny<string>(), filename))
                .Returns(new[] {filename});

            loader.FileLoaders.Add(mockLoader.Object);
            // The container should call the load method
            // with the given filename
            string path = string.Empty;

            var emptyActions = new Action<IServiceContainer>[] {};
            mockLoader.Expect(l => l.CanLoad(filename)).Returns(true);
            mockLoader.Expect(l => l.Load(filename)).Returns(emptyActions);

            loader.LoadDirectory(path, filename);
            loader.LoadInto(mockContainer.Object);

            mockLoader.VerifyAll();
            mockListing.VerifyAll();
        }

        [Test]
        public void LoaderMustRevertToDefaultsAfterReset()
        {
            var mockFileLoader = new Mock<IActionLoader<IServiceContainer, string>>();
            var mockPlugin = new Mock<ILoaderPlugin<IServiceContainer>>();
            var loader = new Loader();

            // Fill the loader with random data
            loader.FileLoaders.Add(mockFileLoader.Object);
            loader.Plugins.Add(mockPlugin.Object);
            loader.QueuedActions.Add(container => Console.WriteLine("Hello, World"));
            loader.CustomLoaderActions.Add(targetLoader => Console.WriteLine("Hello, World"));

            // Reset the loader and make sure everything was cleared
            loader.Reset();

            Assert.IsTrue(loader.FileLoaders.Count == 0);
            Assert.IsTrue(loader.Plugins.Count == 0);
            Assert.IsTrue(loader.QueuedActions.Count == 0);
            Assert.IsTrue(loader.CustomLoaderActions.Count == 0);
        }

        [Test]
        public void LoaderMustSignalToPluginsWhenTheLoadBeginsAndEnds()
        {
            var mockPlugin = new Mock<ILoaderPlugin<IServiceContainer>>();
            var container = new Mock<IServiceContainer>();
            var mockListing = new Mock<IDirectoryListing>();
            var loader = new Loader<IServiceContainer>();

            // Setup the directory listing and            
            // return an empty listing since
            // we're only interested in the plugin behavior
            mockListing.Expect(listing => listing.GetFiles(It.IsAny<string>(), string.Empty))
                .Returns(new string[0]);

            // Initialize the loader
            loader.DirectoryLister = mockListing.Object;
            loader.Plugins.Add(mockPlugin.Object);

            // Both the BeginLoad and EndLoad methods should be called
            mockPlugin.Expect(p => p.BeginLoad(container.Object));
            mockPlugin.Expect(p => p.EndLoad(container.Object));

            // Execute the loader
            loader.LoadDirectory(string.Empty, string.Empty);
            loader.LoadInto(container.Object);

            mockPlugin.VerifyAll();
            mockListing.VerifyAll();
        }

        [Test]
        public void TypeExtractorMustListTypesFromGivenAssembly()
        {
            Assembly targetAssembly = typeof(SampleClass).Assembly;

            ITypeExtractor extractor = new TypeExtractor();
            IEnumerable<Type> results = extractor.GetTypes(targetAssembly);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() > 0);
        }
    }
}