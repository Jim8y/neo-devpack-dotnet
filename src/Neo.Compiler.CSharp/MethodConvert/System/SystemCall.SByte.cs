// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.SByte.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Handles sbyte.Parse(string) method call.
    /// </summary>
    private static void HandleSByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.Dup();
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.LeadingZeroCount(sbyte) method call.
    /// Returns the number of leading zero bits in the binary representation.
    /// </summary>
    private static void HandleSByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
        
        // Check if value is negative (return 0 for negative values)
        methodConvert.Dup(); // a a
        methodConvert.Push0(); // a a 0
        methodConvert.Jump(OpCode.JMPGE, notNegative); // a
        methodConvert.Drop();
        methodConvert.Push0();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        notNegative.Instruction = methodConvert.Nop();
        methodConvert.Push(0); // count value 0
        
        // Count leading zeros by shifting right until value becomes 0
        loopStart.Instruction = methodConvert.Swap(); // 0 value
        methodConvert.Dup(); // 0 value value
        methodConvert.Push0(); // 0 value value 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop); // 0 value
        methodConvert.Push1(); // 0 value 1
        methodConvert.ShR(); // 0 (value >> 1)
        methodConvert.Swap(); // (value >> 1) 0
        methodConvert.Inc(); // (value >> 1) 1
        methodConvert.Jump(OpCode.JMP, loopStart);
        
        endLoop.Instruction = methodConvert.Drop();
        methodConvert.Push(8);
        methodConvert.Swap();
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.CopySign(sbyte, sbyte) method call.
    /// Returns a value with the magnitude of x and the sign of y.
    /// </summary>
    private static void HandleSByteCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        
        // Check for overflow
        JumpTarget noOverflowTarget = new();
        methodConvert.Dup();
        methodConvert.Push(sbyte.MaxValue);
        methodConvert.Jump(OpCode.JMPLE, noOverflowTarget);
        methodConvert.Throw();
        noOverflowTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.CreateChecked&lt;T&gt;(T) method call.
    /// Creates an sbyte from a value, throwing an exception if the value is out of range.
    /// </summary>
    private static void HandleSByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        methodConvert.Dup();
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(new BigInteger(sbyte.MaxValue) + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.CreateSaturating&lt;T&gt;(T) method call.
    /// Creates an sbyte from a value, clamping the result to the sbyte range.
    /// </summary>
    private static void HandleSByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue);
        
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        
        // Validate min <= max
        methodConvert.Dup(); // value min max max
        methodConvert.Rot(); // value max max min
        methodConvert.Dup(); // value max max min min
        methodConvert.Rot(); // value max min min max
        methodConvert.Jump(OpCode.JMPLT, exceptionTarget); // value max min
        methodConvert.Throw();
        
        exceptionTarget.Instruction = methodConvert.Nop();
        methodConvert.Rot(); // max min value
        methodConvert.Dup(); // max min value value
        methodConvert.Rot(); // max value value min
        methodConvert.Dup(); // max value value min min
        methodConvert.Rot(); // max value min min value
        methodConvert.Jump(OpCode.JMPGT, minTarget); // max value min
        
        methodConvert.Drop(); // max value
        methodConvert.Dup(); // max value value
        methodConvert.Rot(); // value value max
        methodConvert.Dup(); // value value max max
        methodConvert.Rot(); // value max max value
        methodConvert.Jump(OpCode.JMPLT, maxTarget); // value max
        methodConvert.Drop();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        minTarget.Instruction = methodConvert.Nop();
        methodConvert.Reverse3();
        methodConvert.Drop();
        methodConvert.Drop();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        maxTarget.Instruction = methodConvert.Nop();
        methodConvert.Swap();
        methodConvert.Drop();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.RotateLeft(sbyte, int) method call.
    /// Rotates the bits of a value left by a specified number of positions.
    /// </summary>
    private static void HandleSByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        
        // public static sbyte RotateLeft(sbyte value, int rotateAmount) => 
        //     (sbyte)((value << (rotateAmount & 7)) | ((byte)value >> ((8 - rotateAmount) & 7)));
        var bitWidth = sizeof(sbyte) * 8;
        
        methodConvert.Push(bitWidth - 1);  // Push 7 (8-bit - 1)
        methodConvert.And();    // rotateAmount & 7
        methodConvert.Swap();
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.Shl();    // value << (rotateAmount & 7)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.And();    // Ensure SHL result is 8-bit
        
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.And();
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);  // Push 8
        methodConvert.Swap();   // Swap top two elements
        methodConvert.Sub();    // 8 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 7
        methodConvert.And();    // (8 - rotateAmount) & 7
        methodConvert.ShR();    // (byte)value >> ((8 - rotateAmount) & 7)
        methodConvert.Or();
        
        // Convert to signed byte
        methodConvert.Dup();    // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 7 (0x80)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);
        methodConvert.Push(BigInteger.One << bitWidth); // BigInteger.One << 8 (0x100)
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.RotateRight(sbyte, int) method call.
    /// Rotates the bits of a value right by a specified number of positions.
    /// </summary>
    private static void HandleSByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        
        // public static sbyte RotateRight(sbyte value, int rotateAmount) => 
        //     (sbyte)(((value & 0xFF) >> (rotateAmount & 7)) | ((value & 0xFF) << ((8 - rotateAmount) & 7)));
        var bitWidth = sizeof(sbyte) * 8;
        
        methodConvert.Push(bitWidth - 1);  // Push 7 (8-bit - 1)
        methodConvert.And();    // rotateAmount & 7
        methodConvert.Push(bitWidth);
        methodConvert.Mod();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Swap();
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.Shl();    // value << (rotateAmount & 7)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.And();    // Ensure SHL result is 8-bit
        
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.And();
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);
        methodConvert.Mod();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth);  // Push 8
        methodConvert.Swap();   // Swap top two elements
        methodConvert.Sub();    // 8 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 7
        methodConvert.And();    // (8 - rotateAmount) & 7
        methodConvert.ShR();    // (byte)value >> ((8 - rotateAmount) & 7)
        methodConvert.Or();
        
        // Convert to signed byte
        methodConvert.Dup();    // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 7 (0x80)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);
        methodConvert.Push(BigInteger.One << bitWidth); // BigInteger.One << 8 (0x100)
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles sbyte.PopCount(sbyte) method call.
    /// Returns the number of set bits in the binary representation.
    /// </summary>
    private static void HandleSBytePopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        // Determine bit width of sbyte
        var bitWidth = sizeof(sbyte) * 8;

        // Mask to ensure the value is treated as an 8-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // 0xFF
        methodConvert.And(); // value = value & 0xFF
        
        // Initialize count to 0
        methodConvert.Push(0); // value count
        methodConvert.Swap(); // count value
        
        // Loop to count the number of 1 bits
        JumpTarget loopStart = new();
        JumpTarget endLoop = new();
        
        loopStart.Instruction = methodConvert.Dup(); // count value value
        methodConvert.Push0(); // count value value 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop); // count value
        methodConvert.Dup(); // count value value
        methodConvert.Push1(); // count value value 1
        methodConvert.And(); // count value (value & 1)
        methodConvert.Rot(); // value (value & 1) count
        methodConvert.Add(); // value count += (value & 1)
        methodConvert.Swap(); // count value
        methodConvert.Push1(); // count value 1
        methodConvert.ShR(); // count value >>= 1
        methodConvert.Jump(OpCode.JMP, loopStart);

        endLoop.Instruction = methodConvert.Drop(); // Drop the remaining value
    }
}