using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_String : DebugAndTestBase<Contract_String>
    {
        [TestMethod]
        public void Test_TestSubstring()
        {
            var log = new List<string>();
            TestEngine.OnRuntimeLogDelegate method = (UInt160 sender, string msg) =>
            {
                log.Add(msg);
            };

            Contract.OnRuntimeLog += method;
            Contract.TestSubstring();
            Assert.AreEqual(3075900, Engine.FeeConsumed.Value);
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(2, log.Count);
            Assert.AreEqual("1234567", log[0]);
            Assert.AreEqual("1234", log[1]);
        }

        [TestMethod]
        public void Test_TestMain()
        {
            var log = new List<string>();
            TestEngine.OnRuntimeLogDelegate method = (UInt160 sender, string msg) =>
            {
                log.Add(msg);
            };

            Contract.OnRuntimeLog += method;
            Contract.TestMain();
            Assert.AreEqual(7625310, Engine.FeeConsumed.Value);
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("Hello, Mark ! Current timestamp is 1468595301000.", log[0]);
        }

        [TestMethod]
        public void Test_TestEqual()
        {
            var log = new List<string>();
            TestEngine.OnRuntimeLogDelegate method = (UInt160 sender, string msg) =>
            {
                log.Add(msg);
            };

            Contract.OnRuntimeLog += method;
            Contract.TestEqual();
            Assert.AreEqual(1970970, Engine.FeeConsumed.Value);
            Contract.OnRuntimeLog -= method;

            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("True", log[0]);
        }

        [TestMethod]
        public void Test_TestEmpty()
        {
            Assert.AreEqual("", Contract.TestEmpty());
            Assert.AreEqual(984270, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestIsNullOrEmpty()
        {
            Assert.IsTrue(Contract.TestIsNullOrEmpty(""));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);

            Assert.IsTrue(Contract.TestIsNullOrEmpty(null));
            Assert.AreEqual(1047300, Engine.FeeConsumed.Value);

            Assert.IsFalse(Contract.TestIsNullOrEmpty("hello world"));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestStringNull()
        {
            var res = Contract.TestStringNull("Hello world");
            Assert.AreEqual(1047360, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.TestStringNull(null));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
        }
    }
}
