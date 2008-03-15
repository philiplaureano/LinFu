using System;
using System.Collections.Generic;
using System.Text;
using NMock2;
using NUnit.Framework;

namespace LinFu.Reflection.Tests
{
    [TestFixture]
    public abstract class BaseFixture
    {
        protected Mockery mock;
        [SetUp]
        public void Init()
        {
            mock = new Mockery();

            OnInit();
        }
        [TearDown]
        public void Term()
        {
            mock.VerifyAllExpectationsHaveBeenMet();

            OnTerm();
        }

        protected virtual void OnInit() { }
        protected virtual void OnTerm() { }
    }
}
