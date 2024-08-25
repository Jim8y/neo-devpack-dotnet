// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal static partial class SystemMethods
{
    private static void HandleShortParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsShortCheck();
    }

    // HandleShortLeadingZeroCount

    private static void HandleShortLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
        sb.Dup(); // a a
        sb.Push0();// a a 0
        sb.JmpGe(notNegative); //a
        sb.Drop();
        sb.Push0();
        sb.Jmp(endTarget);
        notNegative.Instruction = sb.Nop();
        sb.Push0(); // count 5 0
        sb.Swap().SetTarget(loopStart); //0 5
        sb.Dup();//  0 5 5
        sb.Push0();// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.ShR(1); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        sb.Drop().SetTarget(endLoop);
        sb.Push16();
        sb.Swap();
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    // HandleShortCopySign
    private static void HandleShortCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget nonZeroTarget = new();
        JumpTarget nonZeroTarget2 = new();
        // a b
        sb.Sign();         // a 1
        sb.Dup(); // a 1 1
        sb.Push0(); // a 1 1 0
        sb.JmpLt(nonZeroTarget); // a 1
        sb.Drop();
        sb.Push1(); // a 1
        nonZeroTarget.Instruction = sb.Nop(); // a 1
        sb.Swap();         // 1 a
        sb.Dup();// 1 a a
        sb.Sign();// 1 a 0
        sb.Dup();// 1 a 0 0
        sb.Push0(); // 1 a 0 0 0
        sb.JmpLt(nonZeroTarget2); // 1 a 0
        sb.Drop();
        sb.Push1();
        sb.SetTarget(nonZeroTarget2); // 1 a 1
        sb.Rot();// a 1 1
        sb.Equal();// a 1 1
        JumpTarget endTarget = new();
        sb.JmpIf(endTarget); // a
        sb.Negate();
        sb.SetTarget(endTarget);

        var endTarget2 = new JumpTarget();
        sb.Dup();
        sb.Push(short.MinValue);
        sb.Push(new BigInteger(short.MaxValue) + 1);
        sb.AddInstruction(OpCode.WITHIN);
        sb.Jump(OpCode.JMPIF, endTarget2);
        sb.Throw();
        endTarget2.Instruction = sb.Nop();
    }

    // HandleShortCreateChecked
    private static void HandleShortCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsShortCheck();
    }

    // HandleShortCreateSaturating
    private static void HandleShortCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(short.MinValue);
        sb.Push(short.MaxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        sb.Dup();// 5 0 10 10
        sb.Rot();// 5 10 10 0
        sb.Dup();// 5 10 10 0 0
        sb.Rot();// 5 10 0 0 10
        sb.JmpLt(exceptionTarget);// 5 10 0
        sb.Throw();
        sb.SetTarget(exceptionTarget);
        sb.Rot();// 10 0 5
        sb.Dup();// 10 0 5 5
        sb.Rot();// 10 5 5 0
        sb.Dup();// 10 5 5 0 0
        sb.Rot();// 10 5 0 0 5
        sb.JmpGt(minTarget);// 10 5 0
        sb.Drop();// 10 5
        sb.Dup();// 10 5 5
        sb.Rot();// 5 5 10
        sb.Dup();// 5 5 10 10
        sb.Rot();// 5 10 10 5
        sb.JmpLt(maxTarget);// 5 10
        sb.Drop();
        sb.Jmp(endTarget);
        sb.SetTarget(minTarget);
        sb.Reverse3();
        sb.Drop();
        sb.Drop();
        sb.Jmp(endTarget);
        sb.SetTarget(maxTarget);
        sb.Swap();
        sb.Drop();
        sb.SetTarget(endTarget);
    }

    // implement HandleShortRotateLeft
    private static void HandleShortRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static short RotateLeft(short value, int rotateAmount) => (short)((value << (rotateAmount & 15)) | ((ushort)value >> ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(short) * 8;
        sb.And(bitWidth - 1);    // rotateAmount & 15
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 15)
        sb.And((BigInteger.One << bitWidth) - 1); // Ensure SHL result is 16-bit
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 16
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 16 - rotateAmount
        sb.And(bitWidth - 1);    // (16 - rotateAmount) & 15
        sb.ShR();    // (ushort)value >> ((16 - rotateAmount) & 15)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 16 (0x10000)
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    // HandleShortRotateRight
    private static void HandleShortRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static short RotateRight(short value, int rotateAmount) => (short)((value >> (rotateAmount & 15)) | ((ushort)value << ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(short) * 8;
        sb.And(bitWidth - 1);    // rotateAmount & 15
        sb.Push(bitWidth);
        sb.Mod();
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 15)
        sb.And((BigInteger.One << bitWidth) - 1); // Ensure SHL result is 16-bit
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);
        sb.Mod();
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.Push(bitWidth);  // Push 16
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 16 - rotateAmount
        sb.And(bitWidth - 1);    // (16 - rotateAmount) & 15
        sb.ShR();    // (ushort)value >> ((16 - rotateAmount) & 15)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 16 (0x10000)
        sb.Sub();
        sb.SetTarget(endTarget);
    }
}
