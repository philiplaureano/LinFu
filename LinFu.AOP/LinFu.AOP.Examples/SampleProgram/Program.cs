using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleLibrary;

using LinFu.AOP;
using LinFu.AOP.Interfaces;
namespace SampleProgram
{
    public class SpeakProvider : BaseMethodReplacementProvider
    {
        protected override bool ShouldReplace(IInvocationContext context)
        {
            // Only replace the Speak() method
            return context.TargetMethod.Name == "Speak" && 
                context.TargetMethod.DeclaringType == typeof(Person);
        }
        protected override IMethodReplacement GetReplacement(IInvocationContext context)
        {
            return new SpeakMethodReplacement();
        }
    }
    public class AroundSpeakMethod : IAroundInvoke     
    {
        #region IAroundInvoke Members

        public void AfterInvoke(IInvocationContext context, object returnValue)
        {
            // Do nothing
        }

        public void BeforeInvoke(IInvocationContext context)
        {
            Console.WriteLine("AroundSpeakMethod: Hello, CodeProject!");
        }

        #endregion
    }
    public class SpeakMethodReplacement : IMethodReplacement
    {
        public object Invoke(IInvocationContext context)
        {
           // Say "hi" to CodeProject
           Console.WriteLine("SpeakMethodReplacement: Hello, CodeProject!");

           // Say “Hello, World!” by reusing the original implementation
           return context.TargetMethod.Invoke(context.Target, context.Arguments);
        }
    }
    public static class ObjectExtensions
    {
        public static T As<T>(this object target)
            where T : class
        {
            T result = null;

            if (target is T)
                result = target as T;

            return result;
        }
        public static void EnableInterception(this object target)
        {
            IModifiableType modified = target.As<IModifiableType>();
            if (modified == null)
                return;

            modified.IsInterceptionEnabled = true;
        }
    }
    // Note: The MSBuild Project file for SampleLibrary in this example has 
    // been modified to use LinFu.AOP only in Debug builds, so if
    // you run this program using the Release configuration,
    // SampleLibrary.dll will not be modified by LinFu.
    public static class Program
    {
        public static void Main()
        {
            RunInstanceMethodReplacementDemo();
            RunInstanceBasedAroundInvokeInjectionDemo();
            RunClassBasedMethodReplacementDemo();
            RunClassBasedAroundInvokeInjectionDemo();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private static void RunClassBasedAroundInvokeInjectionDemo()
        {
            var provider = new SimpleAroundInvokeProvider(new AroundSpeakMethod(),
                    c => c.TargetMethod.Name == "Speak");

            AroundInvokeRegistry.Providers.Add(provider);
            var first = new Person();
            first.EnableInterception();
            Console.WriteLine("First Person: ");
            first.Speak();

            Console.WriteLine("Second Person: ");
            var second = new Person();
            second.EnableInterception();
            second.Speak();
        }

        private static void RunClassBasedMethodReplacementDemo()
        {
            // Surround the Speak() method regardless
            // of which instance is used
            var provider = new SpeakProvider();
            MethodReplacementRegistry.Providers.Add(provider);

            // NOTE: For performance reasons, interception is disabled by default
            Console.WriteLine("First Person: ");
            Person first = new Person();
            first.EnableInterception();

            first.Speak();

            Console.WriteLine("Second Person: ");
            Person second = new Person();
            second.EnableInterception();

            second.Speak();
        }

        private static void RunInstanceBasedAroundInvokeInjectionDemo()
        {
            Person person = new Person();
            IModifiableType modified = person.As<IModifiableType>();

            if (modified != null)
            {
                var provider = new SimpleAroundInvokeProvider(new AroundSpeakMethod(), 
                    c => c.TargetMethod.Name == "Speak");

                modified.IsInterceptionEnabled = true;
                modified.AroundInvokeProvider = provider;
            }
            // Say hello to CP using an IAroundInvoke instance
            person.Speak();
        }

        private static void RunInstanceMethodReplacementDemo()
        {
            Person person = new Person();

            IModifiableType modified = null;

            // HACK: Prevent the compiler from checking
            // if the person instance implements IModifiedType
            object personRef = person;
            if (personRef is IModifiableType)
                modified = personRef as IModifiableType;

            if (modified != null)
            {
                // Enable interception for this instance
                modified.IsInterceptionEnabled = true;
                modified.MethodReplacementProvider = new SpeakProvider();
            }

            // Say “hello” to both the world, and CodeProject
            person.Speak();
        }
    }

}
