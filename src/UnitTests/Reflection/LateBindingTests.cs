using LinFu.IoC.Reflection;
using Moq;
using Xunit;
    
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.Reflection
{
    public class LateBindingTests
    {
        [Fact]
        public void ShouldCallLateBoundGenericMethodWithFourGenericArguments()
        {
            var mock = new Mock<ISampleServiceWithGenericMethods>();
            mock.Expect(m => m.SomeMethod<object, object, object, object>());

            var targetInstance = mock.Object;
            targetInstance.Invoke<object, object, object, object>("SomeMethod");

            mock.VerifyAll();
        }

        [Fact]
        public void ShouldCallLateBoundGenericMethodWithThreeGenericArguments()
        {
            var mock = new Mock<ISampleServiceWithGenericMethods>();
            mock.Expect(m => m.SomeMethod<object, object, object>());

            var targetInstance = mock.Object;
            targetInstance.Invoke<object, object, object>("SomeMethod");

            mock.VerifyAll();
        }

        [Fact]
        public void ShouldCallLateBoundGenericMethodWithTwoGenericArguments()
        {
            var mock = new Mock<ISampleServiceWithGenericMethods>();
            mock.Expect(m => m.SomeMethod<object, object>());

            var targetInstance = mock.Object;
            targetInstance.Invoke<object, object>("SomeMethod");

            mock.VerifyAll();
        }

        [Fact]
        public void ShouldCallLateBoundMethod()
        {
            var mock = new Mock<ISampleService>();
            mock.Expect(m => m.DoSomething());

            var targetInstance = mock.Object;
            targetInstance.Invoke("DoSomething");

            mock.VerifyAll();
        }

        [Fact]
        public void ShouldCallLateBoundMethodWithGenericMethod()
        {
            var mock = new Mock<ISampleServiceWithGenericMethods>();
            mock.Expect(m => m.SomeMethod<object>());

            var targetInstance = mock.Object;
            targetInstance.Invoke<object>("SomeMethod");

            mock.VerifyAll();
        }
    }
}