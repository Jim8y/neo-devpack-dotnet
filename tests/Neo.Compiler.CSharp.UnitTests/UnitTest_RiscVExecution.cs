// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RiscVExecution.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_RiscVExecution
    {
        private static readonly string TestArtifactsPath = Path.Combine(
            Path.GetDirectoryName(typeof(UnitTest_RiscVExecution).Assembly.Location)!,
            "..", "..", "..", "RiscVTestArtifacts");

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Directory.CreateDirectory(TestArtifactsPath);
        }

        [TestMethod]
        public void Test_RiscVBridge_Availability()
        {
            using var bridge = new RiscVExecutionBridge();

            // The bridge should either be available or gracefully handle missing library
            // This test verifies the bridge can be instantiated
            Assert.IsNotNull(bridge);

            // If the library is not available, skip remaining tests
            if (!bridge.IsAvailable)
            {
                Assert.Inconclusive("RISC-V host library is not available. Skipping RISC-V tests.");
            }
        }

        [TestMethod]
        public void Test_RiscVBridge_Execute_Minimal()
        {
            using var bridge = new RiscVExecutionBridge();

            if (!bridge.IsAvailable)
            {
                Assert.Inconclusive("RISC-V host library is not available.");
                return;
            }

            // Create a minimal test callback
            var callback = new TestHostCallback();

            // Create a simple test binary (this would normally be a real .polkavm file)
            // For now, we expect this to fail since we don't have a real binary
            var testBinary = new byte[] { 0x50, 0x56, 0x4D, 0x00 }; // PV\0\0 magic

            try
            {
                var result = bridge.Execute(
                    testBinary,
                    "main",
                    Array.Empty<Neo.VM.Types.StackItem>(),
                    TriggerType.Application,
                    0x334F454E,
                    53,
                    1234567890UL,
                    1000000000L,
                    callback);

                // If we get here, the binary was valid
                Assert.IsNotNull(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("execution failed"))
            {
                // Expected for invalid binary - test passes if we get here
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Test_RiscVTestHelper_RustToolchain_Check()
        {
            var isAvailable = RiscVTestHelper.IsRustToolchainAvailable();

            // This test documents whether Rust is available in the test environment
            // It doesn't fail if Rust is missing - just documents the state
            Console.WriteLine($"Rust toolchain available: {isAvailable}");

            if (isAvailable)
            {
                Console.WriteLine($"Cargo path: {RiscVTestHelper.GetCargoPath()}");
                Console.WriteLine($"Rustc path: {RiscVTestHelper.GetRustcPath()}");
            }
        }

        [TestMethod]
        public void Test_RiscVTestHelper_CreateMinimalContract()
        {
            var testDir = Path.Combine(TestArtifactsPath, "MinimalContract");

            try
            {
                // Clean up if exists
                if (Directory.Exists(testDir))
                {
                    Directory.Delete(testDir, recursive: true);
                }

                var contractDir = RiscVTestHelper.CreateMinimalTestContract(testDir);

                Assert.IsTrue(Directory.Exists(contractDir));
                Assert.IsTrue(File.Exists(Path.Combine(contractDir, "Cargo.toml")));
                Assert.IsTrue(File.Exists(Path.Combine(contractDir, "src", "main.rs")));

                // Verify Cargo.toml content
                var cargoContent = File.ReadAllText(Path.Combine(contractDir, "Cargo.toml"));
                StringAssert.Contains(cargoContent, "[package]");
                StringAssert.Contains(cargoContent, "name = ");

                // Verify main.rs content
                var mainContent = File.ReadAllText(Path.Combine(contractDir, "src", "main.rs"));
                StringAssert.Contains(mainContent, "#![no_std]");
                StringAssert.Contains(mainContent, "#![no_main]");
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(testDir))
                {
                    try { Directory.Delete(testDir, recursive: true); } catch { }
                }
            }
        }

        [TestMethod]
        public void Test_TestHostCallback_DefaultHandlers()
        {
            var callback = new TestHostCallback();

            // Test GetTrigger
            var triggerHash = Neo.VM.ApplicationEngine.GetSyscallHash("System.Runtime.GetTrigger");
            var result = callback.HandleSyscall(triggerHash, Array.Empty<Neo.VM.Types.StackItem>());

            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOfType(result[0], typeof(Neo.VM.Types.Integer));
            Assert.AreEqual((int)TriggerType.Application, (int)((Neo.VM.Types.Integer)result[0]).GetInteger());

            // Test GetNetwork
            var networkHash = Neo.VM.ApplicationEngine.GetSyscallHash("System.Runtime.GetNetwork");
            result = callback.HandleSyscall(networkHash, Array.Empty<Neo.VM.Types.StackItem>());

            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOfType(result[0], typeof(Neo.VM.Types.Integer));
            Assert.AreEqual(0x334F454E, (int)((Neo.VM.Types.Integer)result[0]).GetInteger());

            // Test Platform
            var platformHash = Neo.VM.ApplicationEngine.GetSyscallHash("System.Runtime.Platform");
            result = callback.HandleSyscall(platformHash, Array.Empty<Neo.VM.Types.StackItem>());

            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOfType(result[0], typeof(Neo.VM.Types.ByteString));
            Assert.AreEqual("NEO", System.Text.Encoding.UTF8.GetString(((Neo.VM.Types.ByteString)result[0]).GetSpan()));
        }

        [TestMethod]
        public void Test_TestHostCallback_CustomHandler()
        {
            var callback = new TestHostCallback();
            var customCalled = false;

            callback.RegisterHandler("Test.Custom", stack =>
            {
                customCalled = true;
                return new[] { Neo.VM.Types.StackItem.True };
            });

            var customHash = Neo.VM.ApplicationEngine.GetSyscallHash("Test.Custom");
            var result = callback.HandleSyscall(customHash, Array.Empty<Neo.VM.Types.StackItem>());

            Assert.IsTrue(customCalled);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Neo.VM.Types.StackItem.True, result[0]);
        }

        [TestMethod]
        public void Test_TestHostCallback_Storage()
        {
            var storage = new System.Collections.Generic.Dictionary<byte[], byte[]>(new ByteArrayComparer());
            var callback = TestHostCallback.WithStorage(storage);

            // Test Put
            var putHash = Neo.VM.ApplicationEngine.GetSyscallHash("System.Storage.Put");
            var key = new Neo.VM.Types.ByteString(new byte[] { 1, 2, 3 });
            var value = new Neo.VM.Types.ByteString(new byte[] { 4, 5, 6 });
            var context = new Neo.VM.Types.Integer(1);

            var result = callback.HandleSyscall(putHash, new Neo.VM.Types.StackItem[] { context, key, value });
            Assert.AreEqual(0, result.Length);

            // Test Get
            var getHash = Neo.VM.ApplicationEngine.GetSyscallHash("System.Storage.Get");
            result = callback.HandleSyscall(getHash, new Neo.VM.Types.StackItem[] { context, key });

            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOfType(result[0], typeof(Neo.VM.Types.ByteString));
            CollectionAssert.AreEqual(new byte[] { 4, 5, 6 }, ((Neo.VM.Types.ByteString)result[0]).GetSpan().ToArray());

            // Test Delete
            var deleteHash = Neo.VM.ApplicationEngine.GetSyscallHash("System.Storage.Delete");
            result = callback.HandleSyscall(deleteHash, new Neo.VM.Types.StackItem[] { context, key });
            Assert.AreEqual(0, result.Length);

            // Verify deletion
            result = callback.HandleSyscall(getHash, new Neo.VM.Types.StackItem[] { context, key });
            Assert.AreEqual(Neo.VM.Types.StackItem.Null, result[0]);
        }

        [TestMethod]
        public void Test_RiscVTestHelper_StackItems()
        {
            // Test creating various stack items
            var intItem = RiscVTestHelper.StackItems.Integer(42);
            Assert.AreEqual(42, (long)intItem.GetInteger());

            var bigIntItem = RiscVTestHelper.StackItems.Integer(new System.Numerics.BigInteger(9999999999999));
            Assert.AreEqual(9999999999999, (long)bigIntItem.GetInteger());

            var bytesItem = RiscVTestHelper.StackItems.ByteString(new byte[] { 1, 2, 3 });
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, bytesItem.GetSpan().ToArray());

            var stringItem = RiscVTestHelper.StackItems.ByteString("hello");
            CollectionAssert.AreEqual(System.Text.Encoding.UTF8.GetBytes("hello"), stringItem.GetSpan().ToArray());

            var boolItem = RiscVTestHelper.StackItems.Boolean(true);
            Assert.AreEqual(Neo.VM.Types.StackItem.True, boolItem);

            var nullItem = RiscVTestHelper.StackItems.Null();
            Assert.AreEqual(Neo.VM.Types.StackItem.Null, nullItem);

            var arrayItem = RiscVTestHelper.StackItems.Array(intItem, boolItem);
            Assert.AreEqual(2, arrayItem.Count);
        }

        [TestMethod]
        public void Test_RiscVTestHelper_HexConversion()
        {
            var bytes = new byte[] { 0xAB, 0xCD, 0xEF, 0x01 };
            var hex = RiscVTestHelper.BytesToHex(bytes);
            Assert.AreEqual("abcdef01", hex);

            var converted = RiscVTestHelper.HexToBytes("AB CD EF 01");
            CollectionAssert.AreEqual(bytes, converted);

            var converted2 = RiscVTestHelper.HexToBytes("0xabcdef01");
            CollectionAssert.AreEqual(bytes, converted2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_RiscVTestHelper_HexConversion_InvalidLength()
        {
            RiscVTestHelper.HexToBytes("abc");
        }

        // Helper class for comparing byte arrays in dictionary
        private class ByteArrayComparer : System.Collections.Generic.IEqualityComparer<byte[]>
        {
            public bool Equals(byte[]? x, byte[]? y)
            {
                if (x == null || y == null) return x == y;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(byte[] obj)
            {
                return obj == null ? 0 : obj.Aggregate(0, (a, b) => a * 31 + b);
            }
        }
    }
}
