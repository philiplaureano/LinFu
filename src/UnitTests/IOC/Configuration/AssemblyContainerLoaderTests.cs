using System;
using System.Collections.Generic;
using System.Reflection;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;
using Moq;
using NUnit.Framework;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class AssemblyContainerLoaderTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            _mockAssemblyLoader = new Mock<IAssemblyLoader>();
            _mockTypeExtractor = new Mock<ITypeExtractor>();
            _mockTypeLoader = new Mock<ITypeLoader>();
        }

        [TearDown]
        public void Term()
        {
            _mockAssemblyLoader.VerifyAll();
            _mockTypeExtractor.VerifyAll();
            _mockTypeLoader.VerifyAll();

            _mockAssemblyLoader = null;
            _mockTypeExtractor = null;
            _mockTypeLoader = null;
        }

        #endregion

        private Mock<IAssemblyLoader> _mockAssemblyLoader;
        private Mock<ITypeExtractor> _mockTypeExtractor;
        private Mock<ITypeLoader> _mockTypeLoader;

        [Test]
        public void AssemblyContainerLoaderShouldCallAssemblyLoader()
        {
            var containerLoader = new AssemblyContainerLoader();

            // The container loader should use the assembly loader
            // to load the assembly
            string filename = "input.dll";

            _mockAssemblyLoader.Expect(loader => loader.Load(filename)).Returns(typeof(SampleClass).Assembly);

            containerLoader.AssemblyLoader = _mockAssemblyLoader.Object;
            containerLoader.Load(filename);
        }

        [Test]
        public void AssemblyContainerLoaderShouldCallTypeExtractor()
        {
            var containerLoader = new AssemblyContainerLoader();
            string filename = "input.dll";

            Assembly targetAssembly = typeof(SampleClass).Assembly;

            // Make sure that it calls the assembly loader
            _mockAssemblyLoader.Expect(loader => loader.Load(filename)).Returns(targetAssembly);

            // It must call the Type Extractor
            _mockTypeExtractor.Expect(extractor => extractor.GetTypes(targetAssembly))
                .Returns(new[] { typeof(SampleClass) });

           
            var assemblyActionLoader = new AssemblyActionLoader<IServiceContainer>(() => containerLoader.TypeLoaders);
            assemblyActionLoader.TypeExtractor = _mockTypeExtractor.Object;

            containerLoader.AssemblyLoader = _mockAssemblyLoader.Object;
            containerLoader.AssemblyActionLoader = assemblyActionLoader;
            containerLoader.Load(filename);
        }

        [Test]
        public void AssemblyContainerLoaderShouldCallTypeLoader()
        {
            // HACK: The Cut&Paste is ugly, but it works
            var containerLoader = new AssemblyContainerLoader();
            string filename = "input.dll";

            Assembly targetAssembly = typeof(SampleClass).Assembly;

            // Make sure that it calls the assembly loader
            _mockAssemblyLoader.Expect(loader => loader.Load(filename)).Returns(targetAssembly);

            // It must call the Type Extractor
            _mockTypeExtractor.Expect(extractor => extractor.GetTypes(targetAssembly))
                .Returns(new[] { typeof(SampleClass) });

            // Make sure that it calls the type loaders
            _mockTypeLoader.Expect(loader => loader.CanLoad(typeof(SampleClass))).Returns(true);
            _mockTypeLoader.Expect(loader => loader.Load(typeof(SampleClass)))
                .Returns(new Action<IServiceContainer>[0]);
           
            var assemblyActionLoader = new AssemblyActionLoader<IServiceContainer>(() => containerLoader.TypeLoaders);
            assemblyActionLoader.TypeExtractor = _mockTypeExtractor.Object;

            containerLoader.AssemblyLoader = _mockAssemblyLoader.Object;
            containerLoader.AssemblyActionLoader = assemblyActionLoader;

            // The container loader should call the type loader
            // once the load method is called
            containerLoader.TypeLoaders.Add(_mockTypeLoader.Object);

            containerLoader.Load(filename);
        }

        [Test]
        public void AssemblyContainerLoaderShouldOnlyLoadDllFiles()
        {
            var mockTypeLoader = new Mock<ITypeLoader>();
            var containerLoader = new AssemblyContainerLoader();
            containerLoader.TypeLoaders.Add(mockTypeLoader.Object);

            // This should return true
            string validFile = typeof(AssemblyContainerLoaderTests).Assembly.Location;
            Assert.IsTrue(containerLoader.CanLoad(validFile));

            // This should return false;
            string invalidFile = "input.txt";
            Assert.IsFalse(containerLoader.CanLoad(invalidFile));
        }
    }
}