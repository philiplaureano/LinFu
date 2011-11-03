using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
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
            filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.dll", Guid.NewGuid()));
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
            ModuleDefinition definition = ModuleDefinition.CreateModule("testAssembly", ModuleKind.Dll);

        	Assembly assembly = definition.Assembly.ToAssembly();
            Assert.IsTrue(assembly != null);
        }

        [Test]
        public void CecilShouldExtractSampleClassFromSignedAssembly()
        {
            string location = typeof (SampleHelloClass).Assembly.Location;

            AssemblyDefinition sourceAssembly = AssemblyDefinition.ReadAssembly(location);
            Assert.IsNotNull(sourceAssembly);

            ModuleDefinition targetModule = ModuleDefinition.CreateModule("testAssembly", ModuleKind.Dll);
            AssemblyDefinition definition = targetModule.Assembly;

            foreach (TypeDefinition typeDef in sourceAssembly.MainModule.Types)
            {
                // Copy the source type to the target assembly
            	throw new NotImplementedException();
                //targetModule.Inject(typeDef);
            }

            // Convert the new assemblyDef into an actual assembly
            Assembly assembly = definition.ToAssembly();
            Assert.IsNotNull(assembly);

            Type[] types = assembly.GetTypes();
            Assert.IsTrue(types.Length > 0);

            // The imported type must match the original type
            Type firstType = types.FirstOrDefault();
            Assert.IsNotNull(firstType);
            Assert.AreEqual(firstType.Name, typeof (SampleHelloClass).Name);

            object instance = Activator.CreateInstance(firstType);
            Assert.IsNotNull(instance);

            MethodInfo speakMethod = firstType.GetMethod("Speak");
            Assert.IsNotNull(speakMethod);

            speakMethod.Invoke(instance, new object[] {});
        }

        [Test]
        public void CecilShouldRemoveStrongNameFromAssembly()
        {
            string location = typeof (SampleHelloClass).Assembly.Location;

            AssemblyDefinition sourceAssembly = AssemblyDefinition.ReadAssembly(location);


            Assert.IsNotNull(sourceAssembly);
            sourceAssembly.RemoveStrongName();

            Assembly assembly = sourceAssembly.ToAssembly();
            Assert.IsNotNull(assembly);

            AssemblyName assemblyName = assembly.GetName();

            // The public key should be empty
            byte[] bytes = assemblyName.GetPublicKey();
            Assert.IsTrue(bytes.Length == 0);
            return;
        }

        [Test]
        public void MethodInvokerShouldProperlyHandleReturnValues()
        {
            MethodInfo targetMethod = typeof (object).GetMethod("GetHashCode");
            var instance = new object();

            int hash = instance.GetHashCode();
            container.AddDefaultServices();

            var invoker = container.GetService<IMethodInvoke<MethodInfo>>();
            Assert.IsNotNull(invoker);

            object result = invoker.Invoke(instance, targetMethod, new object[] {});
            Assert.AreEqual(result, hash);
        }
    }
}