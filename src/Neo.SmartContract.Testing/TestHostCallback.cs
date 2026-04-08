// Copyright (C) 2015-2026 The Neo Project.
//
// TestHostCallback.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Default implementation of ITestHostCallback for handling syscalls in RISC-V tests.
    /// </summary>
    public class TestHostCallback : ITestHostCallback
    {
        private readonly Dictionary<uint, Func<StackItem[], StackItem[]>> _handlers = new();
        private readonly Dictionary<uint, string> _syscallNames = new();

        /// <summary>
        /// Creates a new TestHostCallback with default syscall handlers.
        /// </summary>
        public TestHostCallback()
        {
            RegisterDefaultHandlers();
        }

        /// <summary>
        /// Registers a custom syscall handler.
        /// </summary>
        /// <param name="api">The API hash.</param>
        /// <param name="name">Human-readable name for debugging.</param>
        /// <param name="handler">The handler function.</param>
        public void RegisterHandler(uint api, string name, Func<StackItem[], StackItem[]> handler)
        {
            _handlers[api] = handler;
            _syscallNames[api] = name;
        }

        /// <summary>
        /// Registers a handler by syscall name (hashes the name).
        /// </summary>
        /// <param name="syscallName">The syscall name (e.g., "System.Runtime.Log").</param>
        /// <param name="handler">The handler function.</param>
        public void RegisterHandler(string syscallName, Func<StackItem[], StackItem[]> handler)
        {
            var hash = ApplicationEngine.GetSyscallHash(syscallName);
            RegisterHandler(hash, syscallName, handler);
        }

        /// <summary>
        /// Handles a syscall from the RISC-V contract.
        /// </summary>
        public StackItem[] HandleSyscall(uint api, StackItem[] inputStack)
        {
            if (_handlers.TryGetValue(api, out var handler))
            {
                return handler(inputStack);
            }

            if (_syscallNames.TryGetValue(api, out var name))
            {
                throw new NotImplementedException($"Syscall '{name}' (0x{api:x8}) is registered but has no handler.");
            }

            throw new NotSupportedException($"Unknown syscall: 0x{api:x8}");
        }

        /// <summary>
        /// Gets the name of a registered syscall.
        /// </summary>
        public string? GetSyscallName(uint api)
        {
            return _syscallNames.TryGetValue(api, out var name) ? name : null;
        }

        private void RegisterDefaultHandlers()
        {
            // System.Runtime.GetTrigger
            RegisterHandler("System.Runtime.GetTrigger", stack =>
            {
                // Return Application trigger by default
                return new[] { new Integer((int)TriggerType.Application) };
            });

            // System.Runtime.GetNetwork
            RegisterHandler("System.Runtime.GetNetwork", stack =>
            {
                // Return mainnet magic by default
                return new[] { new Integer(0x334F454E) };
            });

            // System.Runtime.GetAddressVersion
            RegisterHandler("System.Runtime.GetAddressVersion", stack =>
            {
                return new[] { new Integer(53) };
            });

            // System.Runtime.GasLeft
            RegisterHandler("System.Runtime.GasLeft", stack =>
            {
                // Return a large value by default
                return new[] { new Integer(1000000000L) };
            });

            // System.Runtime.Platform
            RegisterHandler("System.Runtime.Platform", stack =>
            {
                return new[] { new ByteString(System.Text.Encoding.UTF8.GetBytes("NEO")) };
            });

            // System.Runtime.Log
            RegisterHandler("System.Runtime.Log", stack =>
            {
                if (stack.Length > 0 && stack[^1] is ByteString message)
                {
                    var logMessage = System.Text.Encoding.UTF8.GetString(message.GetSpan());
                    System.Diagnostics.Debug.WriteLine($"[RISC-V Log] {logMessage}");
                    Console.WriteLine($"[RISC-V Log] {logMessage}");
                }
                // Log consumes the message and returns nothing
                return Array.Empty<StackItem>();
            });

            // System.Storage.GetContext
            RegisterHandler("System.Storage.GetContext", stack =>
            {
                // Return a mock storage context
                return new[] { new Integer(1) };
            });

            // System.Storage.GetReadOnlyContext
            RegisterHandler("System.Storage.GetReadOnlyContext", stack =>
            {
                // Return a mock read-only storage context
                return new[] { new Integer(2) };
            });
        }

        /// <summary>
        /// Creates a TestHostCallback with storage support using the provided storage dictionary.
        /// </summary>
        /// <param name="storage">Dictionary to use for storage operations.</param>
        public static TestHostCallback WithStorage(Dictionary<byte[], byte[]> storage)
        {
            var callback = new TestHostCallback();

            callback.RegisterHandler("System.Storage.Get", stack =>
            {
                if (stack.Length < 2) throw new InvalidOperationException("Storage.Get requires context and key");
                if (stack[^1] is not ByteString key) throw new InvalidOperationException("Key must be ByteString");

                var keyBytes = key.GetSpan().ToArray();
                if (storage.TryGetValue(keyBytes, out var value))
                {
                    return Append(stack[..^1], new ByteString(value));
                }
                return Append(stack[..^1], StackItem.Null);
            });

            callback.RegisterHandler("System.Storage.Put", stack =>
            {
                if (stack.Length < 3) throw new InvalidOperationException("Storage.Put requires context, key, and value");
                if (stack[^2] is not ByteString key) throw new InvalidOperationException("Key must be ByteString");
                if (stack[^1] is not ByteString value) throw new InvalidOperationException("Value must be ByteString");

                storage[key.GetSpan().ToArray()] = value.GetSpan().ToArray();
                return stack[..^3];
            });

            callback.RegisterHandler("System.Storage.Delete", stack =>
            {
                if (stack.Length < 2) throw new InvalidOperationException("Storage.Delete requires context and key");
                if (stack[^1] is not ByteString key) throw new InvalidOperationException("Key must be ByteString");

                storage.Remove(key.GetSpan().ToArray());
                return stack[..^2];
            });

            return callback;
        }

        private static StackItem[] Append(StackItem[] inputStack, StackItem item)
        {
            var next = new StackItem[inputStack.Length + 1];
            Array.Copy(inputStack, next, inputStack.Length);
            next[^1] = item;
            return next;
        }
    }
}
