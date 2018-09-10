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
using Xunit;
using SampleStronglyNamedLibrary;

namespace LinFu.UnitTests.Reflection
{
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


        [Fact]
        public void AssemblyDefinitionMustBeConvertibleToActualAssembly()
        {
            var name = new AssemblyNameDefinition("testAssembly", new Version(1, 0));
            var definition = AssemblyDefinition.CreateAssembly(name, "testModule", ModuleKind.Dll);

            var assembly = definition.ToAssembly();
            Assert.True(assembly != null);
        }

        [Fact]
        public void CecilShouldExtractSampleClassFromSignedAssembly()
        {
            var location = typeof(SampleHelloClass).Assembly.Location;

            var sourceAssembly = AssemblyDefinition.ReadAssembly(location);
            Assert.NotNull(sourceAssembly);

            var name = new AssemblyNameDefinition("testAssembly", new Version(1, 0));
            var definition = AssemblyDefinition.CreateAssembly(name, "testModule", ModuleKind.Dll);
            var targetModule = definition.MainModule;
            foreach (TypeDefinition typeDef in sourceAssembly.MainModule.Types)
            {
                // Copy the source type to the target assembly
                targetModule.Types.Add(typeDef);   
            }        

            // Convert the new assemblyDef into an actual assembly
            var assembly = definition.ToAssembly();
            Assert.NotNull(assembly);

            var types = assembly.GetTypes();
            Assert.True(types.Length > 0);

            // The imported type must match the original type
            var firstType = types.FirstOrDefault();
            Assert.NotNull(firstType);
            Assert.Equal(firstType.Name, typeof(SampleHelloClass).Name);

            var instance = Activator.CreateInstance(firstType);
            Assert.NotNull(instance);

            var speakMethod = firstType.GetMethod("Speak");
            Assert.NotNull(speakMethod);

            speakMethod.Invoke(instance, new object[] { });
        }

        [Fact]
        public void CecilShouldRemoveStrongNameFromAssembly()
        {
            var location = typeof(SampleHelloClass).Assembly.Location;

            var sourceAssembly = AssemblyDefinition.ReadAssembly(location);

            Assert.NotNull(sourceAssembly);
            sourceAssembly.RemoveStrongName();

            var assembly = sourceAssembly.ToAssembly();
            Assert.NotNull(assembly);

            var assemblyName = assembly.GetName();

            // The public key should be empty
            var bytes = assemblyName.GetPublicKey();
            Assert.True(bytes.Length == 0);
        }

        [Fact]
        public void MethodInvokerShouldProperlyHandleReturnValues()
        {
            var targetMethod = typeof(object).GetMethod("GetHashCode");
            var instance = new object();

            var hash = instance.GetHashCode();
            container.AddDefaultServices();

            var invoker = container.GetService<IMethodInvoke<MethodInfo>>();
            Assert.NotNull(invoker);

            var result = invoker.Invoke(instance, targetMethod);
            Assert.Equal(result, hash);
        }
    }
}