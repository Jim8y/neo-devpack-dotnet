// Copyright (C) 2015-2026 The Neo Project.
//
// RiscVExecutionBridge.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Runtime.InteropServices;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Bridge for executing RISC-V contracts via P/Invoke to the native host library.
    /// </summary>
    public sealed class RiscVExecutionBridge : IDisposable
    {
        public const string LibraryPathEnvironmentVariable = "NEO_RISCV_HOST_LIB";
        private const string DefaultLibraryPath = "libneo_riscv_host.so";

        private IntPtr _libraryHandle;
        private bool _disposed;

        // Native function delegates
        private delegate bool ExecuteContractDelegate(
            IntPtr binaryPtr,
            nuint binaryLen,
            IntPtr methodPtr,
            nuint methodLen,
            IntPtr initialStackPtr,
            nuint initialStackLen,
            byte trigger,
            uint network,
            byte addressVersion,
            ulong timestamp,
            long gasLeft,
            long execFeeFactorPico,
            IntPtr userData,
            IntPtr hostCallback,
            IntPtr hostFree,
            out NativeExecutionResult result);

        private delegate void FreeExecutionResultDelegate(ref NativeExecutionResult result);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeExecutionResult
        {
            public long FeeConsumedPico;
            public uint State;
            public IntPtr StackPtr;
            public nuint StackLen;
            public IntPtr ErrorPtr;
            public nuint ErrorLen;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeStackItem
        {
            public uint Kind;
            public long IntegerValue;
            public IntPtr BytesPtr;
            public nuint BytesLen;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeHostResult
        {
            public IntPtr StackPtr;
            public nuint StackLen;
            public IntPtr ErrorPtr;
            public nuint ErrorLen;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool HostCallbackDelegate(
            IntPtr userData,
            uint api,
            nuint instructionPointer,
            byte trigger,
            uint networkMagic,
            byte addressVersion,
            ulong persistingTimestamp,
            long gasLeft,
            IntPtr inputStackPtr,
            nuint inputStackLen,
            out NativeHostResult result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void HostFreeCallbackDelegate(IntPtr userData, ref NativeHostResult result);

        private readonly ExecuteContractDelegate? _executeContract;
        private readonly FreeExecutionResultDelegate? _freeExecutionResult;
        private readonly HostCallbackDelegate _hostCallback;
        private readonly HostFreeCallbackDelegate _hostFreeCallback;
        private readonly IntPtr _hostCallbackPtr;
        private readonly IntPtr _hostFreeCallbackPtr;

        /// <summary>
        /// Gets whether the native library is loaded and available.
        /// </summary>
        public bool IsAvailable => _libraryHandle != IntPtr.Zero && _executeContract != null;

        /// <summary>
        /// Creates a new RiscVExecutionBridge, attempting to load the native library.
        /// </summary>
        /// <param name="libraryPath">Optional path to the native library. If null, uses NEO_RISCV_HOST_LIB environment variable or default path.</param>
        public RiscVExecutionBridge(string? libraryPath = null)
        {
            _hostCallback = StaticHostCallback;
            _hostFreeCallback = StaticHostFreeCallback;
            _hostCallbackPtr = Marshal.GetFunctionPointerForDelegate(_hostCallback);
            _hostFreeCallbackPtr = Marshal.GetFunctionPointerForDelegate(_hostFreeCallback);

            libraryPath ??= Environment.GetEnvironmentVariable(LibraryPathEnvironmentVariable) ?? DefaultLibraryPath;

            try
            {
                _libraryHandle = NativeLibrary.Load(libraryPath);
                var executeExport = NativeLibrary.GetExport(_libraryHandle, "neo_riscv_execute_native_contract");
                var freeExport = NativeLibrary.GetExport(_libraryHandle, "neo_riscv_free_execution_result");
                _executeContract = Marshal.GetDelegateForFunctionPointer<ExecuteContractDelegate>(executeExport);
                _freeExecutionResult = Marshal.GetDelegateForFunctionPointer<FreeExecutionResultDelegate>(freeExport);
            }
            catch (Exception ex)
            {
                // Library not found or incompatible - bridge will be unavailable
                System.Diagnostics.Debug.WriteLine($"RISC-V host library not available: {ex.Message}");
                _libraryHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Executes a RISC-V contract binary.
        /// </summary>
        /// <param name="binary">The .polkavm binary data.</param>
        /// <param name="method">The method name to execute.</param>
        /// <param name="initialStack">Initial stack items.</param>
        /// <param name="trigger">Trigger type.</param>
        /// <param name="networkMagic">Network magic number.</param>
        /// <param name="addressVersion">Address version.</param>
        /// <param name="timestamp">Persisting block timestamp.</param>
        /// <param name="gasLeft">Gas limit.</param>
        /// <param name="callback">Callback for handling syscalls.</param>
        /// <returns>Execution result containing state and stack.</returns>
        public RiscVExecutionResult Execute(
            byte[] binary,
            string method,
            StackItem[] initialStack,
            TriggerType trigger,
            uint networkMagic,
            byte addressVersion,
            ulong timestamp,
            long gasLeft,
            ITestHostCallback? callback = null)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("RISC-V host library is not available.");

            if (_executeContract is null || _freeExecutionResult is null)
                throw new InvalidOperationException("Native functions not loaded.");

            var binaryPtr = Marshal.AllocHGlobal(binary.Length);
            var methodBytes = System.Text.Encoding.UTF8.GetBytes(method);
            var methodPtr = Marshal.AllocHGlobal(methodBytes.Length);
            NativeExecutionResult nativeResult = default;
            var callbackState = new HostCallbackState { Callback = callback };
            var callbackHandle = GCHandle.Alloc(callbackState);
            var initialState = CreateNativeStack(initialStack);

            try
            {
                Marshal.Copy(binary, 0, binaryPtr, binary.Length);
                Marshal.Copy(methodBytes, 0, methodPtr, methodBytes.Length);

                if (!_executeContract(
                        binaryPtr,
                        (nuint)binary.Length,
                        methodPtr,
                        (nuint)methodBytes.Length,
                        initialState.StackPtr,
                        initialState.StackLen,
                        (byte)trigger,
                        networkMagic,
                        addressVersion,
                        timestamp,
                        gasLeft,
                        1000, // execFeeFactorPico
                        GCHandle.ToIntPtr(callbackHandle),
                        _hostCallbackPtr,
                        _hostFreeCallbackPtr,
                        out nativeResult))
                {
                    throw new InvalidOperationException("Native RISC-V contract execution failed.");
                }

                var resultStack = ReadStack(nativeResult.StackPtr, nativeResult.StackLen);
                var state = nativeResult.State == 0 ? VMState.HALT : VMState.FAULT;
                var faultMessage = nativeResult.ErrorPtr == IntPtr.Zero
                    ? null
                    : Marshal.PtrToStringUTF8(nativeResult.ErrorPtr, checked((int)nativeResult.ErrorLen));

                return new RiscVExecutionResult(state, resultStack, faultMessage);
            }
            finally
            {
                FreeNativeStack(ref initialState);
                if (nativeResult.StackPtr != IntPtr.Zero)
                {
                    _freeExecutionResult(ref nativeResult);
                }
                if (callbackHandle.IsAllocated)
                {
                    callbackHandle.Free();
                }
                Marshal.FreeHGlobal(methodPtr);
                Marshal.FreeHGlobal(binaryPtr);
            }
        }

        private static bool StaticHostCallback(
            IntPtr userData,
            uint api,
            nuint instructionPointer,
            byte trigger,
            uint networkMagic,
            byte addressVersion,
            ulong persistingTimestamp,
            long gasLeft,
            IntPtr inputStackPtr,
            nuint inputStackLen,
            out NativeHostResult result)
        {
            result = default;
            try
            {
                var handle = GCHandle.FromIntPtr(userData);
                var state = (HostCallbackState)handle.Target!;
                var inputStack = ReadStack(inputStackPtr, inputStackLen);

                if (state.Callback is null)
                {
                    result = CreateErrorResult("No host callback registered.");
                    return true;
                }

                var outputStack = state.Callback.HandleSyscall(api, inputStack);
                result = CreateNativeStack(outputStack);
                return true;
            }
            catch (Exception ex)
            {
                result = CreateErrorResult(ex.Message);
                return true;
            }
        }

        private static void StaticHostFreeCallback(IntPtr userData, ref NativeHostResult result)
        {
            FreeNativeStack(ref result);
        }

        private static NativeHostResult CreateNativeStack(StackItem[] stack)
        {
            if (stack.Length == 0)
                return default;

            var itemSize = Marshal.SizeOf<NativeStackItem>();
            var stackPtr = Marshal.AllocHGlobal(itemSize * stack.Length);

            for (var i = 0; i < stack.Length; i++)
            {
                var nativeItem = stack[i] switch
                {
                    Integer integer => new NativeStackItem
                    {
                        Kind = 0,
                        IntegerValue = (long)integer.GetInteger(),
                        BytesPtr = IntPtr.Zero,
                        BytesLen = 0,
                    },
                    ByteString byteString => CreateByteStringItem(byteString),
                    Neo.VM.Types.Boolean boolean => new NativeStackItem
                    {
                        Kind = 3,
                        IntegerValue = boolean.GetBoolean() ? 1 : 0,
                        BytesPtr = IntPtr.Zero,
                        BytesLen = 0,
                    },
                    Null => new NativeStackItem
                    {
                        Kind = 2,
                        IntegerValue = 0,
                        BytesPtr = IntPtr.Zero,
                        BytesLen = 0,
                    },
                    _ => throw new InvalidOperationException($"Unsupported stack item type: {stack[i].GetType().Name}")
                };

                Marshal.StructureToPtr(nativeItem, IntPtr.Add(stackPtr, i * itemSize), false);
            }

            return new NativeHostResult
            {
                StackPtr = stackPtr,
                StackLen = (nuint)stack.Length,
                ErrorPtr = IntPtr.Zero,
                ErrorLen = 0,
            };
        }

        private static NativeStackItem CreateByteStringItem(ByteString byteString)
        {
            var bytes = byteString.GetSpan().ToArray();
            var bytesPtr = bytes.Length == 0 ? IntPtr.Zero : Marshal.AllocHGlobal(bytes.Length);
            if (bytes.Length > 0)
            {
                Marshal.Copy(bytes, 0, bytesPtr, bytes.Length);
            }

            return new NativeStackItem
            {
                Kind = 1,
                IntegerValue = 0,
                BytesPtr = bytesPtr,
                BytesLen = (nuint)bytes.Length,
            };
        }

        private static void FreeNativeStack(ref NativeHostResult result)
        {
            if (result.StackPtr != IntPtr.Zero)
            {
                var itemSize = Marshal.SizeOf<NativeStackItem>();
                for (var i = 0; i < (int)result.StackLen; i++)
                {
                    var itemPtr = IntPtr.Add(result.StackPtr, i * itemSize);
                    var item = Marshal.PtrToStructure<NativeStackItem>(itemPtr);
                    if (item.BytesPtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(item.BytesPtr);
                    }
                }
                Marshal.FreeHGlobal(result.StackPtr);
                result.StackPtr = IntPtr.Zero;
                result.StackLen = 0;
            }

            if (result.ErrorPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(result.ErrorPtr);
                result.ErrorPtr = IntPtr.Zero;
                result.ErrorLen = 0;
            }
        }

        private static StackItem[] ReadStack(IntPtr stackPtr, nuint stackLen)
        {
            if (stackPtr == IntPtr.Zero || stackLen == 0)
                return Array.Empty<StackItem>();

            var stack = new StackItem[(int)stackLen];
            var itemSize = Marshal.SizeOf<NativeStackItem>();

            for (var i = 0; i < stack.Length; i++)
            {
                var itemPtr = IntPtr.Add(stackPtr, i * itemSize);
                var nativeItem = Marshal.PtrToStructure<NativeStackItem>(itemPtr);
                stack[i] = nativeItem.Kind switch
                {
                    0 => new Integer(nativeItem.IntegerValue),
                    1 => ReadByteString(nativeItem),
                    3 => nativeItem.IntegerValue != 0 ? StackItem.True : StackItem.False,
                    2 => StackItem.Null,
                    _ => throw new InvalidOperationException($"Unsupported native stack item kind: {nativeItem.Kind}")
                };
            }

            return stack;
        }

        private static StackItem ReadByteString(NativeStackItem nativeItem)
        {
            if (nativeItem.BytesPtr == IntPtr.Zero)
                return ByteString.Empty;

            var bytes = new byte[(int)nativeItem.BytesLen];
            Marshal.Copy(nativeItem.BytesPtr, bytes, 0, bytes.Length);
            return new ByteString(bytes);
        }

        private static NativeHostResult CreateErrorResult(string message)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            var errorPtr = bytes.Length == 0 ? IntPtr.Zero : Marshal.AllocHGlobal(bytes.Length);
            if (bytes.Length > 0)
            {
                Marshal.Copy(bytes, 0, errorPtr, bytes.Length);
            }

            return new NativeHostResult
            {
                StackPtr = IntPtr.Zero,
                StackLen = 0,
                ErrorPtr = errorPtr,
                ErrorLen = (nuint)bytes.Length,
            };
        }

        private sealed class HostCallbackState
        {
            public ITestHostCallback? Callback { get; init; }
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_libraryHandle != IntPtr.Zero)
            {
                NativeLibrary.Free(_libraryHandle);
                _libraryHandle = IntPtr.Zero;
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Result of a RISC-V contract execution.
    /// </summary>
    public sealed class RiscVExecutionResult
    {
        /// <summary>
        /// The execution state (HALT or FAULT).
        /// </summary>
        public VMState State { get; }

        /// <summary>
        /// The result stack items.
        /// </summary>
        public StackItem[] Stack { get; }

        /// <summary>
        /// Error message if State is FAULT.
        /// </summary>
        public string? ErrorMessage { get; }

        public RiscVExecutionResult(VMState state, StackItem[] stack, string? errorMessage)
        {
            State = state;
            Stack = stack;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Interface for handling syscalls during RISC-V test execution.
    /// </summary>
    public interface ITestHostCallback
    {
        /// <summary>
        /// Handles a syscall from the RISC-V contract.
        /// </summary>
        /// <param name="api">The API hash of the syscall.</param>
        /// <param name="inputStack">Input stack items.</param>
        /// <returns>Output stack items.</returns>
        StackItem[] HandleSyscall(uint api, StackItem[] inputStack);
    }
}
