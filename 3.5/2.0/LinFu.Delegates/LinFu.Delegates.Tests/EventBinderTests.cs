using System;
using System.Collections.Generic;
using System.Text;
using NMock2;
using NUnit.Framework;

namespace LinFu.Delegates.Tests
{
    [TestFixture]
    public class EventBinderTests
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
        public void ShouldBindToTargetEvent()
        {
            ITestCounter counter = mock.NewMock<ITestCounter>();
            Expect.Once.On(counter).Method("Increment").WithAnyArguments();

            EventSource source = new EventSource();
            CustomDelegate body = delegate
                                      {
                                          counter.Increment();
                                          return null;
                                      };

            EventBinder.BindToEvent("Event1", source, body);
            source.Fire();
        }
    }
}
