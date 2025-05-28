// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Math.cs file belongs to the neo project and is free
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
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Handles Math.Abs methods for various numeric types.
    /// </summary>
    private static void HandleMathAbs(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Abs();
    }

    /// <summary>
    /// Handles Math.Sign methods for various numeric types.
    /// </summary>
    private static void HandleMathSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Sign();
    }

    /// <summary>
    /// Handles Math.Max methods for various numeric types.
    /// </summary>
    private static void HandleMathMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Max();
    }

    /// <summary>
    /// Handles Math.Min methods for various numeric types.
    /// </summary>
    private static void HandleMathMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Min();
    }

    /// <summary>
    /// Handles Math.DivRem method for byte types.
    /// </summary>
    private static void HandleMathByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for sbyte types.
    /// </summary>
    private static void HandleMathSByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for short types.
    /// </summary>
    private static void HandleMathShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for ushort types.
    /// </summary>
    private static void HandleMathUShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for int types.
    /// </summary>
    private static void HandleMathIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for uint types.
    /// </summary>
    private static void HandleMathUIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for long types.
    /// </summary>
    private static void HandleMathLongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem method for ulong types.
    /// </summary>
    private static void HandleMathULongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.Clamp method for various numeric types.
    /// </summary>
    private static void HandleMathClamp(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        var exceptionTarget = new JumpTarget();
        
        // Evaluation stack: value=5 min=0 max=10 <- top
        methodConvert.Over();  // 5 0 10 0
        methodConvert.Over();  // 5 0 10 0 10 <- top
        methodConvert.Jump(OpCode.JMPLE, exceptionTarget);  // 5 0 10  // if 0 <= 10, continue execution
        //methodConvert.Push("min>max");
        methodConvert.Throw();
        
        exceptionTarget.Instruction = methodConvert.Nop();
        methodConvert.Reverse3();  // 10 0 5
        
        // MAX&MIN costs 1<<3 each; 16 Datoshi more expensive at runtime
        methodConvert.Max();  // 10 5
        methodConvert.Min();  // 5
        //methodConvert.Ret();
        
        // Alternatively, a slightly cheaper way at runtime; 10 to 16 Datoshi
        //methodConvert.Over();  // 10 0 5 0
        //methodConvert.Over();  // 10 0 5 0 5
        //methodConvert.Jump(OpCode.JMPGE, minTarget);  // 10 0 5; should return 0 if JMPed
        //methodConvert.Nip();  // 10 5
        //methodConvert.Over();  // 10 5 10
        //methodConvert.Over();  // 10 5 10 5
        //methodConvert.Jump(OpCode.JMPLE, maxTarget);  // 10 5; should return 10 if JMPed
        //methodConvert.Nip();  // 5; should return 5
        //methodConvert.Ret();
        //minTarget.Instruction = methodConvert.Nop();  // 10 0 5; should return 0
        //methodConvert.Drop();  // 10 0; should return 0
        //methodConvert.Nip();  // 0; should return 0
        //methodConvert.Ret();
        //maxTarget.Instruction = methodConvert.Nop();  // 10 5; should return 10
        //methodConvert.Drop();  // 10; should return 10
        //methodConvert.Ret();
    }

    /// <summary>
    /// Handles Math.BigMul(int, int) method call.
    /// </summary>
    private static void HandleMathBigMul(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        JumpTarget endTarget = new();
        methodConvert.Mul();
        methodConvert.Dup();
        methodConvert.Push(long.MinValue);
        methodConvert.Push(new BigInteger(long.MaxValue) + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    // RegisterHandler((double x) => Math.Ceiling(x), HandleMathCeiling);
    // RegisterHandler((double x) => Math.Floor(x), HandleMathFloor);
    // RegisterHandler((double x) => Math.Round(x), HandleMathRound);
    // RegisterHandler((double x) => Math.Truncate(x), HandleMathTruncate);
    // RegisterHandler((double x, double y) => Math.Pow(x, y), HandleMathPow);
    // RegisterHandler((double x) => Math.Sqrt(x), HandleMathSqrt);
    // RegisterHandler((double x) => Math.Log(x), HandleMathLog);
    // RegisterHandler((double x, double y) => Math.Log(x, y), HandleMathLogBase);
}