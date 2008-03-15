using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;

namespace Simple.IoC.Tests
{
    [TestFixture]
    public class CustomizerTests : BaseFixture
    {
        [Test]
        public void ContainerShouldCallCustomizer()
        {
            ICustomizeInstance customizer = mock.NewMock<ICustomizeInstance>();
            Expect.Once.On(customizer).Method("CanCustomize").WithAnyArguments().Will(Return.Value(true));
            Expect.Once.On(customizer).Method("Customize").WithAnyArguments();

            SimpleContainer container = new SimpleContainer();
            TestObject testObject = new TestObject();
            container.AddService<ITestObject>(testObject);
            container.Customizers.Add(customizer);

            ITestObject result = container.GetService<ITestObject>("MyService");
            Assert.AreSame(result, testObject);
        }
    }
}
