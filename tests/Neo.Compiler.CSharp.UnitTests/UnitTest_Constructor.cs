using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Constructor
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Constructor.cs");
        }

        [TestMethod]
        public void TestAirdrop()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("Airdrop", "core-dev");

            Assert.AreEqual(VMState.HALT, _engine.State);
        }
    }
}
