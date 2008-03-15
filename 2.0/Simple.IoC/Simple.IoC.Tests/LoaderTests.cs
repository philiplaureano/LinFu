using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NMock2;
using NUnit.Framework;

using Simple.IoC.Loaders;
namespace Simple.IoC.Tests
{
    [TestFixture]
    public class LoaderTests
    {
        private Mockery mock;
        [SetUp]
        public void Init()
        {
            mock = new Mockery();
        }
        [TearDown]
        public void Term()
        {
            mock.VerifyAllExpectationsHaveBeenMet();
        }
        [Test]
        public void LoaderShouldLoadAssemblyFile()
        {
            // Load the assembly from the current directory
            string fileSpec = "Simple.IoC.Tests.dll";
            string directory = AppDomain.CurrentDomain.BaseDirectory;

            string filename = Path.Combine(directory, fileSpec);
            IContainer mockContainer = mock.NewMock<IContainer>();
            IAssemblyLoader assemblyLoader = mock.NewMock<IAssemblyLoader>();

            // The loader should delegate its calls to the assembly loader
            Expect.AtLeastOnce.On(assemblyLoader).Method("LoadAssembly").With(filename)
                .Will(Return.Value(typeof(ITestObject).Assembly));

            Expect.AtLeastOnce.On(mockContainer).Method("AddFactory").WithAnyArguments();

            Loader loader = new Loader(mockContainer, assemblyLoader);
            loader.FactoryLoader = new CustomFactoryLoader();
            loader.LoadDirectory(directory, fileSpec);
        }
    }
}
