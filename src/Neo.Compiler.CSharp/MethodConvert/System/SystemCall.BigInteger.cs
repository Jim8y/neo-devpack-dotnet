// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.BigInteger.cs file belongs to the neo project and is free
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
    /// Handles BigInteger.One property access.
    /// </summary>
    private static void HandleBigIntegerOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(1);
    }

    /// <summary>
    /// Handles BigInteger.MinusOne property access.
    /// </summary>
    private static void HandleBigIntegerMinusOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(-1);
    }

    /// <summary>
    /// Handles BigInteger.Zero property access.
    /// </summary>
    private static void HandleBigIntegerZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(0);
    }

    /// <summary>
    /// Handles BigInteger.IsZero property access.
    /// </summary>
    private static void HandleBigIntegerIsZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(0);
        methodConvert.NumEqual();
    }

    /// <summary>
    /// Handles BigInteger.IsOne property access.
    /// </summary>
    private static void HandleBigIntegerIsOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(1);
        methodConvert.NumEqual();
    }

    /// <summary>
    /// Handles BigInteger.IsEven property access and IsEvenInteger methods.
    /// </summary>
    private static void HandleBigIntegerIsEven(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(2);
        methodConvert.Mod();
        methodConvert.Not();  // BigInteger GetBoolean() => !value.IsZero;
    }

    /// <summary>
    /// Handles BigInteger.Sign property access.
    /// </summary>
    private static void HandleBigIntegerSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Sign();
    }

    /// <summary>
    /// Handles BigInteger.Pow(BigInteger, int) method call.
    /// </summary>
    private static void HandleBigIntegerPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.POW);
    }

    /// <summary>
    /// Handles BigInteger.ModPow(BigInteger, BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerModPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MODPOW);
    }

    /// <summary>
    /// Handles BigInteger.Add(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerAdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Add();
    }

    /// <summary>
    /// Handles BigInteger.Subtract(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerSubtract(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Sub();
    }

    /// <summary>
    /// Handles BigInteger.Negate(BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerNegate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Negate();
    }

    /// <summary>
    /// Handles BigInteger.Multiply(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerMultiply(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Mul();
    }

    /// <summary>
    /// Handles BigInteger.Divide(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerDivide(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Div();
    }

    /// <summary>
    /// Handles BigInteger.Remainder(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerRemainder(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Mod();
    }

    /// <summary>
    /// Handles BigInteger.Compare(BigInteger, BigInteger) method call.
    /// Returns -1 if left &lt; right, 0 if left == right, 1 if left &gt; right.
    /// </summary>
    private static void HandleBigIntegerCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // if left < right return -1;
        // if left = right return 0;
        // if left > right return 1;
        methodConvert.Sub();
        methodConvert.Sign();
    }

    /// <summary>
    /// Handles BigInteger.GreatestCommonDivisor(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerGreatestCommonDivisor(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        
        JumpTarget gcdTarget = new()
        {
            Instruction = methodConvert.Dup()
        };
        methodConvert.Reverse3();
        methodConvert.Swap();
        methodConvert.Mod();
        methodConvert.Dup();
        methodConvert.Push0();
        methodConvert.NumEqual();
        methodConvert.Jump(OpCode.JMPIFNOT, gcdTarget);
        methodConvert.Drop();
        methodConvert.Abs();
    }

    /// <summary>
    /// Handles BigInteger.ToByteArray() method call.
    /// </summary>
    private static void HandleBigIntegerToByteArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(VM.Types.StackItemType.Buffer);
    }

    /// <summary>
    /// Handles BigInteger.Parse(string) method call with optimization for constant strings.
    /// </summary>
    private static void HandleBigIntegerParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
        {
            if (arguments.Count == 1 && arguments[0] is ArgumentSyntax { NameColon: null } arg)
            {
                // Optimize call when is a constant string
                Optional<object?> constant = model.GetConstantValue(arg.Expression);

                if (constant.HasValue && constant.Value is string strValue && BigInteger.TryParse(strValue, out var bi))
                {
                    // Insert a sequence point for debugging purposes
                    using var sequence = methodConvert.InsertSequencePoint(arg.Expression);
                    methodConvert.Push(bi);
                    return;
                }
            }

            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        }

        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
    }

    /// <summary>
    /// Handles explicit conversion from BigInteger to various numeric types with range checking.
    /// </summary>
    private static void HandleBigIntegerExplicitConversion(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to sbyte.
    /// </summary>
    private static void HandleBigIntegerToSByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to byte.
    /// </summary>
    private static void HandleBigIntegerToByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(byte.MinValue);
        methodConvert.Push(byte.MaxValue + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to short.
    /// </summary>
    private static void HandleBigIntegerToShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(short.MinValue);
        methodConvert.Push(short.MaxValue + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to ushort.
    /// </summary>
    private static void HandleBigIntegerToUShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(ushort.MinValue);
        methodConvert.Push(ushort.MaxValue + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to int.
    /// </summary>
    private static void HandleBigIntegerToInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(int.MinValue);
        methodConvert.Push(new BigInteger(int.MaxValue) + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to uint.
    /// </summary>
    private static void HandleBigIntegerToUInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(uint.MinValue);
        methodConvert.Push(new BigInteger(uint.MaxValue) + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to long.
    /// </summary>
    private static void HandleBigIntegerToLong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(long.MinValue);
        methodConvert.Push(new BigInteger(long.MaxValue) + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to ulong.
    /// </summary>
    private static void HandleBigIntegerToULong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();
        methodConvert.Push(ulong.MinValue);
        methodConvert.Push(new BigInteger(ulong.MaxValue) + 1);
        methodConvert.Within();
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles implicit conversion of various types to BigInteger.
    /// </summary>
    private static void HandleToBigInteger(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    /// <summary>
    /// Handles BigInteger.Max(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Max();
    }

    /// <summary>
    /// Handles BigInteger.Min(BigInteger, BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Min();
    }

    /// <summary>
    /// Handles BigInteger.IsOddInteger method and IsOdd property access.
    /// </summary>
    private static void HandleBigIntegerIsOdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(2);
        methodConvert.Mod();
        methodConvert.Nz();
    }

    /// <summary>
    /// Handles BigInteger.IsNegative property access and IsNegative methods.
    /// </summary>
    private static void HandleBigIntegerIsNegative(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(0);
        methodConvert.Lt();
    }

    /// <summary>
    /// Handles BigInteger.IsPositive property access and IsPositive methods.
    /// </summary>
    private static void HandleBigIntegerIsPositive(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(0);
        methodConvert.Ge();
        // GE instead of GT, because C# BigInteger works like that
        // https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L4098C13-L4098C37
    }

    /// <summary>
    /// Handles BigInteger.IsPow2 method and IsPowerOfTwo property access.
    /// Checks if the value is a power of two.
    /// </summary>
    private static void HandleBigIntegerIsPow2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        // (n & (n-1) == 0) and (n != 0)
        JumpTarget endFalse = new();
        JumpTarget endTrue = new();
        JumpTarget endTarget = new();
        JumpTarget nonZero = new();
        
        methodConvert.Dup();
        methodConvert.Push(0);
        methodConvert.Jump(OpCode.JMPNE, nonZero);
        methodConvert.Drop();
        methodConvert.Jump(OpCode.JMP, endFalse);
        
        nonZero.Instruction = methodConvert.Nop();
        methodConvert.Dup();
        methodConvert.Dec();
        methodConvert.And();
        methodConvert.Push(0);
        methodConvert.Jump(OpCode.JMPEQ, endTrue);
        
        endFalse.Instruction = methodConvert.Nop();
        methodConvert.Push(false);
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        endTrue.Instruction = methodConvert.Nop();
        methodConvert.Push(true);
        
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles BigInteger.Log2(BigInteger) method call.
    /// Returns the base-2 logarithm of the value.
    /// </summary>
    private static void HandleBigIntegerLog2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nonNegativeTarget = new();
        JumpTarget endMethod = new();
        
        methodConvert.Dup(); // value value
        methodConvert.Push0(); // value value 0
        methodConvert.Jump(OpCode.JMPGE, nonNegativeTarget); // value
        methodConvert.Throw();
        
        nonNegativeTarget.Instruction = methodConvert.Nop();
        methodConvert.Dup(); // value value
        methodConvert.Push0(); // value value 0
        methodConvert.Jump(OpCode.JMPEQ, endMethod); // return 0 when input is 0
        methodConvert.Push0(); // value 0
        
        // input = 5 > 0; result = 0; 
        // do
        //   result += 1
        // while (input >> result) > 0
        // result -= 1
        JumpTarget loopStart = new();
        loopStart.Instruction = methodConvert.Nop(); // value result
        methodConvert.Inc(); // value (result + 1)
        methodConvert.Over(); // value (result + 1) value
        methodConvert.Over(); // value (result + 1) value (result + 1)
        methodConvert.ShR(); // value (result + 1) (value >> (result + 1))
        methodConvert.Push0(); // value (result + 1) (value >> (result + 1)) 0
        methodConvert.Jump(OpCode.JMPGT, loopStart); // value (result + 1)
        methodConvert.Nip(); // (result + 1)
        methodConvert.Dec(); // result
        
        endMethod.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles BigInteger.CopySign(BigInteger, BigInteger) method call.
    /// Returns a value with the magnitude of the first argument and the sign of the second.
    /// </summary>
    private static void HandleBigIntegerCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        
        JumpTarget negativeTarget = new();
        JumpTarget endTarget = new();
        
        // Stack: value sign
        // if value == 0 return 0
        // if sign == 0 return abs(value)
        // return value has abs(value) == abs(value), sign(result) == sign(sign)
        methodConvert.Push0(); // value sign 0
        methodConvert.Jump(OpCode.JMPLT, negativeTarget); // value
        methodConvert.Abs();   // abs(value)
        methodConvert.Jump(OpCode.JMP, endTarget);  // abs(value)
        
        negativeTarget.Instruction = methodConvert.Nop();
        methodConvert.Abs();   // abs(value)
        methodConvert.Negate(); // -abs(value)
        
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles BigInteger.DivRem(BigInteger, BigInteger) method call.
    /// Returns both quotient and remainder.
    /// </summary>
    private static void HandleMathBigIntegerDivRem(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        // Stack: dividend divisor -> quotient remainder
        // Perform division
        methodConvert.Dup(); // dividend divisor divisor
        methodConvert.Push(2);
        methodConvert.AddInstruction(OpCode.PICK); // dividend divisor divisor dividend
        methodConvert.Div();  // dividend divisor quotient
        // For types that is restricted by range, there should be quotient <= MaxValue
        // However it's only possible to get quotient == MaxValue + 1 when quotient > MaxValue
        // and it's impossible to get quotient < MinValue
        // Therefore we ignore this case; quotient <= MaxValue is not checked

        // Calculate remainder
        methodConvert.Reverse3();  // quotient dividend divisor
        methodConvert.Mod();  // quotient remainder
        methodConvert.Push(2);
        methodConvert.Pack();
        // It's impossible to get remainder out of range
    }

    /// <summary>
    /// Handles BigInteger.LeadingZeroCount(BigInteger) method call.
    /// Returns the number of leading zero bits.
    /// </summary>
    private static void HandleBigIntegerLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
        
        methodConvert.Dup(); // value value
        methodConvert.Push0(); // value value 0
        methodConvert.Jump(OpCode.JMPGE, notNegative); // value
        methodConvert.Drop();
        methodConvert.Push0();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        notNegative.Instruction = methodConvert.Nop();
        methodConvert.Push(0); // value count
        
        loopStart.Instruction = methodConvert.Swap(); // count value
        methodConvert.Dup(); // count value value
        methodConvert.Push0(); // count value value 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop); // count value
        methodConvert.Push1(); // count value 1
        methodConvert.ShR(); // count (value >> 1)
        methodConvert.Swap(); // (value >> 1) count
        methodConvert.Inc(); // (value >> 1) (count + 1)
        methodConvert.Jump(OpCode.JMP, loopStart);
        
        endLoop.Instruction = methodConvert.Drop();
        methodConvert.Push(256);
        methodConvert.Swap();
        methodConvert.Sub();
        
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles BigInteger.CreateChecked&lt;T&gt;(T) method call.
    /// </summary>
    private static void HandleBigIntegerCreatedChecked(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    /// <summary>
    /// Handles BigInteger.CreateSaturating&lt;T&gt;(T) method call.
    /// </summary>
    private static void HandleBigIntegerCreateSaturating(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
    }

    /// <summary>
    /// Handles BigInteger.Equals(BigInteger) method call.
    /// </summary>
    private static void HandleBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.NumEqual();
    }

    /// <summary>
    /// Handles BigInteger.PopCount(BigInteger) method call.
    /// Returns the number of set bits in the binary representation.
    /// </summary>
    private static void HandleBigIntegerPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Check if the value is within int range
        methodConvert.Dup();
        methodConvert.Within(int.MinValue, int.MaxValue);
        var endIntCheck = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endIntCheck);

        // If within int range, mask with 0xFFFFFFFF
        methodConvert.Push(0xFFFFFFFF);
        methodConvert.And();
        var endMask = new JumpTarget();
        methodConvert.Jump(OpCode.JMP, endMask);

        // If larger than int, throw exception, cause too many check will make the script too long.
        endIntCheck.Instruction = methodConvert.Nop();
        methodConvert.Push("Value out of range, must be between int.MinValue and int.MaxValue.");
        methodConvert.Throw();
        
        endMask.Instruction = methodConvert.Nop();

        // Initialize count to 0
        methodConvert.Push(0); // value count
        methodConvert.Swap(); // count value
        
        // Loop to count the number of 1 bit
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