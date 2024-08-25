using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_DebugInfo
    {
        [TestMethod]
        public void Test_DebugInfo()
        {
            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract_Event.cs").FullName;
            var results = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.Basic,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(testContractsPath);

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);

            var debugInfo = results[0].CreateDebugInformation();
            var nef = results[0].CreateExecutable();

            Assert.AreEqual(nef.Script.Span.ToScriptHash().ToString(), debugInfo["hash"]!.GetString());
            Assert.IsTrue(debugInfo.ContainsProperty("documents"));
            Assert.IsInstanceOfType(debugInfo["documents"], typeof(JArray));
            Assert.IsTrue((debugInfo["documents"] as JArray)!.Count > 0);
            Assert.IsTrue((debugInfo["documents"] as JArray)!.All(n => n is JString), "All documents items should be string!");
            Assert.IsTrue(debugInfo.ContainsProperty("methods"));
            Assert.IsInstanceOfType(debugInfo["methods"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["methods"] as JArray)!.Count);
            Assert.AreEqual("Neo.Compiler.CSharp.TestContracts.Contract_Event,test", (debugInfo["methods"] as JArray)![0]!["name"]!.AsString());
            Assert.AreEqual("0[0]19:28-19:29;1[0]19:13-19:29;2[0]19:13-19:29;3[0]19:13-19:29;4[0]20:28-20:32;5[0]20:13-20:32;6[0]20:13-20:32;7[0]20:13-20:32;8[0]21:13-21:86;9[0]21:13-21:86;10[0]21:25-21:47;15[0]21:25-21:47;17[0]21:13-21:86;18[0]21:13-21:86;19[0]21:49-21:71;24[0]21:49-21:71;26[0]21:13-21:86;27[0]21:13-21:86;28[0]21:73-21:85;30[0]21:13-21:86;31[0]21:13-21:86;41[0]21:13-21:86;46[0]22:9-22:10",
                string.Join(';', ((debugInfo["methods"] as JArray)![0]!["sequence-points"] as JArray)!.Select(u => u!.AsString())));
            Assert.IsTrue(debugInfo.ContainsProperty("events"));
            Assert.IsInstanceOfType(debugInfo["events"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["events"] as JArray)!.Count);
            Assert.AreEqual("Neo.Compiler.CSharp.TestContracts.Contract_Event,Transferred", (debugInfo["events"] as JArray)![0]!["name"]!.AsString());
            Assert.AreEqual("arg1,ByteArray,0;arg2,ByteArray,1;arg3,Integer,2", string.Join(';', ((debugInfo["events"] as JArray)![0]!["params"] as JArray)!.Select(u => u!.AsString())));
            Assert.IsTrue(debugInfo.ContainsProperty("static-variables"));
            Assert.AreEqual("MyStaticVar1,Integer,0;MyStaticVar2,Boolean,1", string.Join(';', (debugInfo["static-variables"] as JArray)!.Select(u => u!.AsString())));
        }
    }
}
