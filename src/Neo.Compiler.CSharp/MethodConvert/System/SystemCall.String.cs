// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.String.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Handles string indexer (string[int]) access.
    /// </summary>
    private static void HandleStringPickItem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.PickItem();
    }

    /// <summary>
    /// Handles string.Length property access.
    /// </summary>
    private static void HandleStringLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Size();
    }

    /// <summary>
    /// Handles string.Contains(string) method call.
    /// </summary>
    private static void HandleStringContains(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Push0();
        methodConvert.Ge();
    }

    /// <summary>
    /// Handles string.IndexOf(string) method call.
    /// </summary>
    private static void HandleStringIndexOf(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    /// <summary>
    /// Handles string.EndsWith(string) method call.
    /// </summary>
    private static void HandleStringEndsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        
        var endTarget = new JumpTarget();
        var validCountTarget = new JumpTarget();
        
        methodConvert.Dup();
        methodConvert.Size();
        methodConvert.Rot();
        methodConvert.Dup();
        methodConvert.Size();
        methodConvert.Dup();
        methodConvert.Push(3);
        methodConvert.AddInstruction(OpCode.ROLL);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Dup();
        methodConvert.Push(0);
        methodConvert.Jump(OpCode.JMPGT, validCountTarget);
        methodConvert.Drop();
        methodConvert.Drop();
        methodConvert.Drop();
        methodConvert.Drop();
        methodConvert.PushF();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        validCountTarget.Instruction = methodConvert.Nop();
        methodConvert.Push(3);
        methodConvert.AddInstruction(OpCode.ROLL);
        methodConvert.Reverse3();
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.Equal();
        
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles string.Substring(int, int) method call.
    /// </summary>
    private static void HandleStringSubstring(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.SubStr();
    }

    /// <summary>
    /// Handles string.Substring(int) method call.
    /// </summary>
    private static void HandleStringSubStringToEnd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Over();
        methodConvert.Size();
        methodConvert.Over();
        methodConvert.Sub();
        methodConvert.SubStr();
    }

    /// <summary>
    /// Handles string.IsNullOrEmpty(string) method call.
    /// </summary>
    private static void HandleStringIsNullOrEmpty(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        
        JumpTarget endTarget = new();
        JumpTarget nullOrEmptyTarget = new();
        
        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.Jump(OpCode.JMPIF, nullOrEmptyTarget);
        methodConvert.Size();
        methodConvert.Push(0);
        methodConvert.NumEqual();
        methodConvert.Jump(OpCode.JMP, endTarget);
        
        nullOrEmptyTarget.Instruction = methodConvert.Drop(); // drop the duped item
        methodConvert.PushT();
        
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles object.Equals(object) method call.
    /// </summary>
    private static void HandleObjectEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Equal();
    }

    /// <summary>
    /// Handles string comparison methods.
    /// </summary>
    private static void HandleStringCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Sub();
        methodConvert.Sign();
    }

    /// <summary>
    /// Handles bool.ToString() method call.
    /// </summary>
    private static void HandleBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        
        JumpTarget trueTarget = new(), endTarget = new();
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);
        methodConvert.Push("False");
        methodConvert.Jump(OpCode.JMP_L, endTarget);
        trueTarget.Instruction = methodConvert.Push("True");
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles char.ToString() method call.
    /// </summary>
    private static void HandleCharToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    /// <summary>
    /// Handles object.ToString() method call.
    /// </summary>
    private static void HandleObjectToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    /// <summary>
    /// Handles numeric types' ToString() methods.
    /// </summary>
    private static void HandleToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
    }

    /// <summary>
    /// Handles string.ToString() method call.
    /// </summary>
    private static void HandleStringToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
    }

    /// <summary>
    /// Handles string.Concat(string, string) method call.
    /// </summary>
    private static void HandleStringConcat(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var firstNotNull = new JumpTarget();
        var secondNotNull = new JumpTarget();
        
        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.JumpIfNot(firstNotNull);
        methodConvert.Drop();
        methodConvert.Push("");
        
        firstNotNull.Instruction = methodConvert.Nop();
        methodConvert.Swap();
        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.JumpIfNot(secondNotNull);
        methodConvert.Drop();
        methodConvert.Push("");
        
        secondNotNull.Instruction = methodConvert.Nop();
        methodConvert.Cat();
    }

    /// <summary>
    /// Handles string.ToLower() method call.
    /// </summary>
    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToLower(methodConvert);
    }

    /// <summary>
    /// Handles string.ToUpper() method call.
    /// </summary>
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToUpper(methodConvert);
    }

    /// <summary>
    /// Handles string.Trim() method call.
    /// </summary>
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);       // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);   // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);     // endIndex = string.Length - 1

        // loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex); // pick a char to check
        CheckWithinWhiteSpace(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();

        // done processing leading whitespace, start processing trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex); // pick a char to check
        CheckWithinWhiteSpace(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();

        // get the substring
        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        GetEndIndex(methodConvert, endIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.Sub();
        methodConvert.Inc();
        methodConvert.SubStr(); // Get the substring up to the last non-space character
    }

    /// <summary>
    /// Handles string.Trim(char) method call.
    /// </summary>
    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);       // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);   // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);     // endIndex = string.Length - 1
        methodConvert.Drop();
        
        // loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex); // pick a char to check
        CheckTrimChar(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();

        // done processing leading whitespace, start processing trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex); // pick a char to check
        CheckTrimChar(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();

        // get the substring
        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        GetEndIndex(methodConvert, endIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.Sub();
        methodConvert.Inc();
        methodConvert.SubStr(); // Get the substring up to the last non-space character
    }

    /// <summary>
    /// Handles string.Replace(string, string) method call.
    /// </summary>
    private static void HandleStringReplace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var replaceStart = new JumpTarget();
        var replaceEnd = new JumpTarget();

        // Duplicate the original string
        methodConvert.Dup();

        // Start of the loop to find the substring
        loopStart.Instruction = methodConvert.Nop();

        // Check if the string contains the substring
        methodConvert.Dup();
        methodConvert.Dup();
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Dup();
        methodConvert.PushM1();
        methodConvert.Equal();
        methodConvert.Jump(OpCode.JMPIF, loopEnd);

        // Get the index of the substring
        methodConvert.Dup();
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);

        // Replace the substring with the new value
        replaceStart.Instruction = methodConvert.Nop();
        methodConvert.Dup();
        methodConvert.AddInstruction(OpCode.LDARG2);
        methodConvert.Cat();
        methodConvert.Dup();
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.Size();
        methodConvert.Add();
        methodConvert.SubStr();
        methodConvert.Cat();
        replaceEnd.Instruction = methodConvert.Nop();

        // Continue the loop
        methodConvert.Jump(OpCode.JMP, loopStart);

        // End of the loop
        loopEnd.Instruction = methodConvert.Nop();
        methodConvert.Drop();
    }

    /// <summary>
    /// Handles string.IndexOf(char) method call.
    /// </summary>
    private static void HandleStringIndexOfChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    /// <summary>
    /// Converts a string to uppercase.
    /// </summary>
    private static void ConvertToUpper(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push(""); // Create an empty ByteString

        methodConvert.Push0(); // Push the initial index (0)
        loopStart.Instruction = methodConvert.Nop();

        methodConvert.Dup(); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Size(); // Get the length of the string
        methodConvert.Lt(); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.Dup(); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Swap();
        methodConvert.PickItem(); // Get the character at the current index
        methodConvert.Dup(); // Duplicate the character
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.Push((ushort)'z' + 1); // Push 'z' + 1
        methodConvert.Within(); // Check if character is within 'a' to 'z'
        methodConvert.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.Cat(); // Append the original character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = methodConvert.Nop();
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.Sub(); // Subtract 'a' from the character
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.Add(); // Add 'A' to the result
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.Cat(); // Append the upper case character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.Nop();
        methodConvert.Drop();
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    /// <summary>
    /// Converts a string to lowercase.
    /// </summary>
    private static void ConvertToLower(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push(""); // Create an empty ByteString

        methodConvert.Push0(); // Push the initial index (0)
        loopStart.Instruction = methodConvert.Nop();

        methodConvert.Dup(); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Size(); // Get the length of the string
        methodConvert.Lt(); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.Dup(); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Swap();
        methodConvert.PickItem(); // Get the character at the current index
        methodConvert.Dup(); // Duplicate the character
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.Push((ushort)'Z' + 1); // Push 'Z' + 1
        methodConvert.Within(); // Check if character is within 'A' to 'Z'
        methodConvert.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.Cat(); // Append the original character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = methodConvert.Nop();
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.Sub(); // Subtract 'A' from the character
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.Add(); // Add 'a' to the result
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.Cat(); // Append the lowercase character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.Nop();
        methodConvert.Drop();
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    // Helper methods for string trimming operations
    private static void InitStrLen(MethodConvert methodConvert, byte strLen)
    {
        GetString(methodConvert);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, strLen);
    }

    private static void GetStrLen(MethodConvert methodConvert, byte strLen) => methodConvert.AccessSlot(OpCode.LDLOC, strLen);

    private static void InitStartIndex(MethodConvert methodConvert, byte startIndex)
    {
        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);
    }

    private static void InitEndIndex(MethodConvert methodConvert, byte endIndex, byte strLen)
    {
        GetStrLen(methodConvert, strLen);
        methodConvert.Dec(); // len-1
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);
    }

    private static void GetEndIndex(MethodConvert methodConvert, byte endIndex) => methodConvert.AccessSlot(OpCode.LDLOC, endIndex);

    private static void GetString(MethodConvert methodConvert) => methodConvert.AddInstruction(OpCode.LDARG0); // Load the string

    private static void GetStartIndex(MethodConvert methodConvert, byte startIndex) => methodConvert.AccessSlot(OpCode.LDLOC, startIndex);

    private static void CheckStartIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte startIndex, byte strLen)
    {
        GetStartIndex(methodConvert, startIndex);
        GetStrLen(methodConvert, strLen);
        methodConvert.Lt(); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void MoveStartIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte startIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, startIndex);
        methodConvert.Inc();
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);
        methodConvert.Jump(OpCode.JMP, loopStart);
    }

    private static void PickCharStart(MethodConvert methodConvert, byte startIndex)
    {
        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.PickItem(); // Get the character at the current index
    }

    private static void MoveEndIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte endIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, endIndex);
        methodConvert.Dec();
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);
        methodConvert.Jump(OpCode.JMP, loopStart);
    }

    private static void CheckWithinWhiteSpace(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.Dup();
        methodConvert.Push((ushort)'\t');
        methodConvert.Push((ushort)'\r' + 1);
        methodConvert.Within(); // check if '\t' <= c <= '\r'
        methodConvert.Swap();

        methodConvert.Push((ushort)' '); // Push space character
        methodConvert.Equal(); // Check if character is a space
        methodConvert.BoolOr(); // check if '\t' <= c <= '\r' or ' ' == c

        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void CheckTrimChar(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.NumEqual();
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void CheckEndIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte endIndex, byte startIndex)
    {
        GetEndIndex(methodConvert, endIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.Gt(); // Check if index > start
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void PickCharEnd(MethodConvert methodConvert, byte endIndex)
    {
        GetString(methodConvert);
        GetEndIndex(methodConvert, endIndex);
        methodConvert.PickItem(); // Get the character at the current index
    }
}