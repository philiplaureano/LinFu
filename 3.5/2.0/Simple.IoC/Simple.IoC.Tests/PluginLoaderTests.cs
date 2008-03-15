using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Simple.IoC.Loaders;

namespace Simple.IoC.Tests
{
    [TestFixture]
    public class PluginLoaderTests : BaseFixture
    {
        protected string directory = Path.GetDirectoryName(typeof(PluginLoaderTests).Assembly.Location);
        protected override void OnInit()
        {
            // TODO: Insert your initialization code here
        }

        protected override void OnTerm()
        {
            // TODO: Insert your cleanup code here
        }

        [Test]
        public void ShouldLoadTestPlugin()
        {
            IContainer container = mock.NewMock<IContainer>();
            Loader loader = new Loader(container);
            
            string fileSpec = "Simple.IoC.Tests.dll";            
            string filename = Path.Combine(directory, fileSpec);

            LoadPluginStrategy strategy = new LoadPluginStrategy();
            
            TestPlugin.Reset();
            loader.LoadStrategy = strategy;
            loader.LoadDirectory(directory, fileSpec);

            Assert.IsTrue(TestPlugin.IsCalled);
        }
    }
}
