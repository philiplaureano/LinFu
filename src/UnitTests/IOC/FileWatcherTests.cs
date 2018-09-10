using System;
using System.IO;
using System.Threading;
using LinFu.IoC;
using Xunit;
using SampleLibrary;

namespace LinFu.UnitTests.IOC
{
    public class FileWatcherTests
    {
        [Fact]
        public void ShouldLoadAssemblyIntoLoaderAtRuntime()
        {
            var path = Path.Combine(@"..\..\..\SampleFileWatcherLibrary\bin\Debug",
                AppDomain.CurrentDomain.BaseDirectory);
            var targetFile = "SampleFileWatcherLibrary.dll";
            var sourceFileName = Path.Combine(path, targetFile);

            var container = new ServiceContainer();
            container.AutoLoadFrom(AppDomain.CurrentDomain.BaseDirectory, "dummy.dll");

            // There should be nothing loaded at this point since the assembly hasn't
            // been copied to the target directory yet
            Assert.False(container.Contains(typeof(ISampleService)));

            // Copy the assembly to the target directory
            // and watch for changes
            var targetFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dummy.dll");
            File.Copy(sourceFileName, targetFileName, true);

            // Give the watcher thread enough time to load the assembly into memory
            Thread.Sleep(500);
            Assert.True(container.Contains(typeof(ISampleService)));

            var instance = container.GetService<ISampleService>();
            Assert.NotNull(instance);

            var typeName = instance.GetType().Name;
            Assert.Equal("SampleFileWatcherServiceClassAddedAtRuntime", typeName);
        }
    }
}