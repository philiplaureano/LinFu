using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Cecil;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
using LinFu.Reflection.Emit.Interfaces;
using Mono.Cecil;
using Moq;
using NUnit.Framework;
using SampleStronglyNamedLibrary;

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
            filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.dll", Guid.NewGuid().ToString()));
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
            var targetMethod = typeof(object).GetMethod("GetHashCode");
            var instance = new object();

            var hash = instance.GetHashCode();
            container.AddDefaultServices();

            var invoker = container.GetService<IMethodInvoke<MethodInfo>>();
            Assert.IsNotNull(invoker);

            var result = invoker.Invoke(instance, targetMethod, new object[] { });
            Assert.AreEqual(result, hash);
        }

        [Test]
        public void CecilShouldExtractSampleClassFromSignedAssembly()
        {
            var location = typeof(SampleHelloClass).Assembly.Location;

            var sourceAssembly = AssemblyFactory.GetAssembly(location);
            Assert.IsNotNull(sourceAssembly);

            var definition = AssemblyFactory.DefineAssembly("testAssembly", AssemblyKind.Dll);
            var targetModule = definition.MainModule;
            foreach (TypeDefinition typeDef in sourceAssembly.MainModule.Types)
            {
                // Copy the source type to the target assembly
                targetModule.Inject(typeDef);
            }

            // Convert the new assemblyDef into an actual assembly
            var assembly = definition.ToAssembly();
            Assert.IsNotNull(assembly);

            var types = assembly.GetTypes();
            Assert.IsTrue(types.Length > 0);

            // The imported type must match the original type
            var firstType = types.FirstOrDefault();
            Assert.IsNotNull(firstType);
            Assert.AreEqual(firstType.Name, typeof(SampleHelloClass).Name);

            var instance = Activator.CreateInstance(firstType);
            Assert.IsNotNull(instance);

            var speakMethod = firstType.GetMethod("Speak");
            Assert.IsNotNull(speakMethod);

            speakMethod.Invoke(instance, new object[] { });
        }

        [Test]
        public void CecilShouldRemoveStrongNameFromAssembly()
        {
            var location = typeof(SampleHelloClass).Assembly.Location;

            var sourceAssembly = AssemblyFactory.GetAssembly(location);


            Assert.IsNotNull(sourceAssembly);
            sourceAssembly.RemoveStrongName();

            var assembly = sourceAssembly.ToAssembly();
            Assert.IsNotNull(assembly);

            var assemblyName = assembly.GetName();
            
            // The public key should be empty
            var bytes = assemblyName.GetPublicKey();
            Assert.IsTrue(bytes.Length == 0);
            return;
        }
    }
}
