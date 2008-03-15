using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NMock2;
using NUnit.Framework;

namespace LinFu.Delegates.Tests
{
    [TestFixture]
    public class ClosureTests
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
        public void ShouldBeAbleToCallDelegate()
        {
            object source = new object();
            EventArgs args = new EventArgs();

            ITestCounter counter = mock.NewMock<ITestCounter>();
            Expect.Once.On(counter).Method("Increment").WithAnyArguments();

            EventHandler dummyHandler = delegate(object eventSource, EventArgs e)
                                            {
                                                // The arguments passed to the
                                                // delegate should match the ones
                                                // given
                                                Assert.AreSame(eventSource, source);
                                                Assert.AreSame(e, args);
                                                counter.Increment();
                                            };

            Closure closure = new Closure(dummyHandler);
            Assert.AreSame(dummyHandler, closure.Target);
            closure.Invoke(source, args);
        }
        [Test]
        public void ShouldBeAbleToCurryArguments()
        {
            object source = new object();
            EventArgs args = new EventArgs();

            ITestCounter counter = mock.NewMock<ITestCounter>();
            Expect.Once.On(counter).Method("Increment").WithAnyArguments();

            EventHandler dummyHandler = delegate(object eventSource, EventArgs e)
                                            {
                                                // The arguments passed to the
                                                // delegate should match the ones
                                                // given
                                                Assert.AreSame(eventSource, source);
                                                Assert.AreSame(e, args);
                                                counter.Increment();
                                            };

            Closure closure = new Closure(dummyHandler, source);
            Assert.IsTrue(closure.Arguments.Contains(source));
            closure.Invoke(args);
        }
        [Test]
        public void ShouldBeAbleToNestClosures()
        {
            MathOperation add = delegate(int a, int b) { return a + b; };
            Closure doMath = new Closure(add, new Closure(add, new Closure(add, 1, 1), 1), 1);

            int result = (int) doMath.Invoke();
            Assert.IsTrue(result == 4);
        }
        [Test]
        public void CurriedClosureShouldBeAbleToBindToAStronglyTypedDelegate()
        {
            MathOperation add = delegate(int a, int b) { return a + b; };
            Closure doMath = new Closure(add, Args.Lambda, 1);
            SingleOperatorMathOperation mathOp = doMath.AdaptTo<SingleOperatorMathOperation>();

            // return 5 + 1
            int result = mathOp(5);
            // The result should be 6
            Assert.AreEqual(6, result);
        }
    }
}
