using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
using LinFu.Reflection.Emit.Interfaces;
using Mono.Cecil;
using Moq;
using NUnit.Framework;

namespace LinFu.UnitTests.Reflection
{
    [TestFixture]
    public class ReflectionEmitTests : BasePEVerifyTestCase
    {
        private Loader loader;
        private ServiceContainer container;
        private string filename;
        protected override void OnInit()
        {
            if (File.Exists(filename))
                File.Delete(filename);

            // Initialize the loader
            // with all of the default LinFu plugins
            loader = new Loader();
            container = new ServiceContainer();
            filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.dll");
            AutoDelete(filename);

            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
            loader.LoadInto(container);
        }
        protected override void OnTerm()
        {
            if (!File.Exists(filename))
                return;

            PEVerify(filename);
        }


        [Test]
        public void AssemblyDefinitionMustBeConvertibleToActualAssembly()
        {
            var definition = AssemblyFactory.DefineAssembly("testAssembly", AssemblyKind.Dll);

            Assembly assembly = definition.ToAssembly();
            Assert.IsTrue(assembly != null);
        }

        [Test]
        public void MethodInvokerShouldProperlyHandleReturnValues()
        {
            var targetMethod = typeof (object).GetMethod("GetHashCode");
            var instance = new object();

            var hash = instance.GetHashCode();
            container.AddDefaultServices();

            var invoker = container.GetService<IMethodInvoke<MethodInfo>>();
            Assert.IsNotNull(invoker);

            var result = invoker.Invoke(instance, targetMethod, new object[]{});
            Assert.AreEqual(result, hash);
        }
    }
}
