// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler.Optimizer;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        #region Fields

        private readonly CompilationContext _context;
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
        private bool _internalInline;
        private bool _initslot;
        private readonly Dictionary<IParameterSymbol, byte> _parameters = new(SymbolEqualityComparer.Default);
        private readonly List<(ILocalSymbol, byte)> _variableSymbols = new();
        private readonly Dictionary<ILocalSymbol, byte> _localVariables = new(SymbolEqualityComparer.Default);
        private readonly List<byte> _anonymousVariables = new();
        private int _localsCount;
        private readonly Stack<List<ILocalSymbol>> _blockSymbols = new();
        private readonly List<Instruction> _instructions = new();
        private readonly JumpTarget _startTarget = new();
        private readonly Dictionary<ILabelSymbol, JumpTarget> _labels = new(SymbolEqualityComparer.Default);
        private readonly Stack<JumpTarget> _continueTargets = new();
        private readonly Stack<JumpTarget> _breakTargets = new();
        private readonly JumpTarget _returnTarget = new();
        private readonly Stack<ExceptionHandling> _tryStack = new();
        private readonly Stack<byte> _exceptionStack = new();
        private readonly Stack<(SwitchLabelSyntax, JumpTarget)[]> _switchStack = new();
        private readonly Stack<bool> _checkedStack = new();

        #endregion

        #region Properties

        public IMethodSymbol Symbol { get; }
        public SyntaxNode? SyntaxNode { get; private set; }
        public IReadOnlyList<Instruction> Instructions => _instructions;
        public IReadOnlyList<(ILocalSymbol Symbol, byte SlotIndex)> Variables => _variableSymbols;
        public bool IsEmpty => _instructions.Count == 0
            || (_instructions.Count == 1 && _instructions[^1].OpCode == OpCode.RET)
            || (_instructions.Count == 2 && _instructions[^1].OpCode == OpCode.RET && _instructions[0].OpCode == OpCode.INITSLOT);

        /// <summary>
        /// captured local variable/parameter symbols when converting current method
        /// </summary>
        public HashSet<ISymbol> CapturedLocalSymbols { get; } = new(SymbolEqualityComparer.Default);

        #endregion

        #region Constructors

        public MethodConvert(CompilationContext context, IMethodSymbol symbol)
        {
            this.Symbol = symbol;
            this._context = context;
            this._checkedStack.Push(context.Options.Checked);
        }

        #endregion

        #region Variables

        private byte AddLocalVariable(ILocalSymbol symbol)
        {
            byte index = (byte)(_localVariables.Count + _anonymousVariables.Count);
            _variableSymbols.Add((symbol, index));
            _localVariables.Add(symbol, index);
            if (_localsCount < index + 1)
                _localsCount = index + 1;
            _blockSymbols.Peek().Add(symbol);
            return index;
        }

        private byte AddAnonymousVariable()
        {
            byte index = (byte)(_localVariables.Count + _anonymousVariables.Count);
            _anonymousVariables.Add(index);
            if (_localsCount < index + 1)
                _localsCount = index + 1;
            return index;
        }

        private void RemoveAnonymousVariable(byte index)
        {
            if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
                _anonymousVariables.Remove(index);
        }

        private void RemoveLocalVariable(ILocalSymbol symbol)
        {
            if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
                _localVariables.Remove(symbol);
        }

        #endregion

        #region Instructions
        private Instruction AddInstruction(Instruction instruction)
        {
            _instructions.Add(instruction);
            return instruction;
        }

        private Instruction AddInstruction(OpCode opcode)
        {
            return AddInstruction(new Instruction
            {
                OpCode = opcode
            });
        }

        private SequencePointInserter InsertSequencePoint(SyntaxNodeOrToken? syntax)
        {
            return new SequencePointInserter(_instructions, syntax);
        }

        private SequencePointInserter InsertSequencePoint(SyntaxReference? syntax)
        {
            return new SequencePointInserter(_instructions, syntax);
        }

        private SequencePointInserter InsertSequencePoint(Location? location)
        {
            return new SequencePointInserter(_instructions, location);
        }

        #endregion

        #region Convert
        public void Convert(SemanticModel model)
        {
            if (Symbol.IsExtern || Symbol.ContainingType.DeclaringSyntaxReferences.IsEmpty)
            {
                if (Symbol.Name == "_initialize")
                {
                    ProcessStaticFields(model);
                    if (_context.StaticFieldCount > 0)
                    {
                        _instructions.Insert(0, new Instruction
                        {
                            OpCode = OpCode.INITSSLOT,
                            Operand = new[] { (byte)_context.StaticFieldCount }
                        });
                    }
                }
                else
                {
                    ConvertExtern();
                }
            }
            else
            {
                if (!Symbol.DeclaringSyntaxReferences.IsEmpty)
                    SyntaxNode = Symbol.DeclaringSyntaxReferences[0].GetSyntax();
                switch (Symbol.MethodKind)
                {
                    case MethodKind.Constructor:

                        if (SyntaxNode is ClassDeclarationSyntax { TypeParameterList: not null } classDeclarationSyntax &&
                            classDeclarationSyntax.TypeParameterList.Parameters.Count != 0)
                        {
                            ProcessFields(model, classDeclarationSyntax.TypeParameterList.Parameters);
                        }
                        else
                        {
                            ProcessFields(model);
                        }
                        ProcessConstructorInitializer(model);
                        break;
                    case MethodKind.StaticConstructor:
                        ProcessStaticFields(model);
                        break;
                    default:
                        if (Symbol.Name.StartsWith("_") && !Symbol.IsInternalCoreMethod())
                            throw new CompilationException(Symbol, DiagnosticId.InvalidMethodName, $"The method name {Symbol.Name} is not valid.");
                        break;
                }
                var modifiers = ConvertModifier(model).ToArray();
                ConvertSource(model);
                if (Symbol.MethodKind == MethodKind.StaticConstructor && _context.StaticFieldCount > 0)
                {
                    _instructions.Insert(0, new Instruction
                    {
                        OpCode = OpCode.INITSSLOT,
                        Operand = new[] { (byte)_context.StaticFieldCount }
                    });
                }
                if (_initslot)
                {
                    byte pc = (byte)_parameters.Count;
                    byte lc = (byte)_localsCount;
                    if (IsInstanceMethod(Symbol)) pc++;
                    if (pc > 0 || lc > 0)
                    {
                        _instructions.Insert(0, new Instruction
                        {
                            OpCode = OpCode.INITSLOT,
                            Operand = new[] { lc, pc }
                        });
                    }
                }
                foreach (var (fieldIndex, attribute) in modifiers)
                {
                    var disposeInstruction = ExitModifier(model, fieldIndex, attribute);
                    if (disposeInstruction is not null && _returnTarget.Instruction is null)
                    {
                        _returnTarget.Instruction = disposeInstruction;
                    }
                }
            }
            if (_returnTarget.Instruction is null)
            {
                if (_instructions.Count > 0 && _instructions[^1].OpCode == OpCode.NOP && _instructions[^1].SourceLocation is not null)
                {
                    _instructions[^1].OpCode = OpCode.RET;
                    _returnTarget.Instruction = _instructions[^1];
                }
                else
                {
                    _returnTarget.Instruction = AddInstruction(OpCode.RET);
                }
            }
            else
            {
                // it comes from modifier clean up
                AddInstruction(OpCode.RET);
            }
            if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
                BasicOptimizer.RemoveNops(_instructions);
            _startTarget.Instruction = _instructions[0];
        }

        /// <summary>
        /// Converts a forwarding method by creating an object of the containing type and jumping to the target method.
        /// </summary>
        /// <param name="model">The semantic model used for compilation.</param>
        /// <param name="target">The target method to which the forwarding method should jump.</param>
        /// <exception cref="CompilationException">
        /// Thrown when the containing type does not have a parameterless constructor.
        /// </exception>
        /// <remarks>
        /// This method performs the following steps to convert a forwarding method:
        ///     1. Retrieves the containing type symbol of the current method.
        ///     2. Creates an object of the containing type using the <see cref="CreateObject"/> method, passing <c>null</c> for the initializer.
        ///     3. Finds the parameterless constructor of the containing type. If no parameterless constructor is found, throws a <see cref="CompilationException"/>.
        ///     4. Calls the parameterless constructor using the <see cref="Call"/> method, passing an empty array of <see cref="ArgumentSyntax"/>.
        ///     5. Sets the return target instruction to jump to the start target of the <paramref name="target"/> method using the <see cref="Jump"/> method.
        ///     6. Sets the start target instruction to the first instruction in the current method.
        /// </remarks>
        public void ConvertForward(SemanticModel model, MethodConvert target)
        {
            INamedTypeSymbol type = Symbol.ContainingType;
            CreateObject(model, type, null);
            IMethodSymbol? constructor = type.InstanceConstructors.FirstOrDefault(p => p.Parameters.Length == 0)
                ?? throw new CompilationException(type, DiagnosticId.NoParameterlessConstructor, "The contract class requires a parameterless constructor.");
            Call(model, constructor, true, Array.Empty<ArgumentSyntax>());
            _returnTarget.Instruction = Jump(OpCode.JMP_L, target._startTarget);
            _startTarget.Instruction = _instructions[0];
        }

        /// <summary>
        /// Processes the initializer for a field.
        /// </summary>
        /// <param name="model">The semantic model.</param>
        /// <param name="field">The field symbol.</param>
        /// <param name="preInitialize">The action to be executed before field initialization.</param>
        /// <param name="postInitialize">The action to be executed after field initialization.</param>
        /// <remarks>
        /// This method handles the initialization of a field based on its attributes and initializer syntax.
        /// If the field has an <see cref="InitialValueAttribute"/> or its subclass attribute, the initial value
        /// is obtained from the attribute and processed according to the specified contract parameter type.
        /// The <see cref="InitialValueAttribute"/> has the highest priority. If an initial value is set in multiple ways
        /// along with <see cref="InitialValueAttribute"/>, the value from <see cref="InitialValueAttribute"/> will be adopted.
        /// If the field doesn't have an initializer attribute, the method attempts to retrieve the initializer
        /// syntax from the field's declaring syntax references or the associated property's syntax.
        /// The method converts the initializer expression and executes the pre-initialization and post-initialization
        /// actions if provided.
        /// </remarks>
        /// <exception cref="CompilationException">
        /// Thrown when an unsupported initial value type is encountered or when an invalid initial value is specified.
        /// </exception>
        private void ProcessFieldInitializer(SemanticModel model, IFieldSymbol field, Action? preInitialize, Action? postInitialize)
        {
            // Check if the field has an InitialValueAttribute or its subclass attribute
            AttributeData? initialValue = field.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(InitialValueAttribute) || p.AttributeClass!.IsSubclassOf(nameof(InitialValueAttribute)));

            if (initialValue is null)
            {
                // Initializer examples:
                //      private int myField = 10; // Field initializer
                //      public string MyProperty { get; set; } = "Hello"; // Property initializer
                EqualsValueClauseSyntax? initializer;
                SyntaxNode syntaxNode;

                // Check if the field does not have any declaring syntax references
                // This can happen when the field is generated by the compiler
                // (e.g., backing field for an auto-implemented property)
                // Example:
                //      public class Person
                //      {
                //           public string Name { get; set; }
                //           public int Age { get; set; }
                //      }
                if (field.DeclaringSyntaxReferences.IsEmpty)
                {
                    // If the field is not associated with a property, return
                    if (field.AssociatedSymbol is not IPropertySymbol property) return;

                    // Get the property declaration syntax from the associated property's declaring syntax references
                    PropertyDeclarationSyntax syntax = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences[0].GetSyntax();
                    syntaxNode = syntax;
                    initializer = syntax.Initializer;
                }
                else
                {
                    // If the field has declaring syntax references, get the variable declarator syntax from the first reference
                    VariableDeclaratorSyntax syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences[0].GetSyntax();
                    syntaxNode = syntax;
                    initializer = syntax.Initializer;
                }

                // If the field or property does not have an initializer, return
                if (initializer is null) return;

                // Get the semantic model for the syntax node's syntax tree
                model = model.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);

                // Insert a sequence point for the syntax node
                using (InsertSequencePoint(syntaxNode))
                {
                    // Invoke the pre-initialization action, if provided
                    preInitialize?.Invoke();

                    // Convert the initializer expression
                    ConvertExpression(model, initializer.Value, syntaxNode);

                    // Invoke the post-initialization action, if provided
                    postInitialize?.Invoke();
                }
            }
            else
            {
                // If the field has an InitialValueAttribute or its subclass attribute
                // Invoke the pre-initialization action, if provided
                preInitialize?.Invoke();

                // Get the initial value from the attribute's constructor argument
                string value = (string)initialValue.ConstructorArguments[0].Value!;

                // Get the attribute name
                var attributeName = initialValue.AttributeClass!.Name;

                // Determine the contract parameter type based on the attribute name
                // This is the old way of setting the initialvalue,
                // user can directly assign them via string now,
                // ref. https://github.com/neo-project/neo-devpack-dotnet/pull/974
                // and analyzer can verify the format, ensuring the correctness of assigned value.
                ContractParameterType parameterType = attributeName switch
                {
                    nameof(InitialValueAttribute) => (ContractParameterType)initialValue.ConstructorArguments[1].Value!,
                    nameof(Hash160Attribute) => ContractParameterType.Hash160,
                    nameof(PublicKeyAttribute) => ContractParameterType.PublicKey,
                    nameof(ByteArrayAttribute) => ContractParameterType.ByteArray,
                    nameof(StringAttribute) => ContractParameterType.String,
                    _ => throw new CompilationException(field, DiagnosticId.InvalidInitialValueType, $"Unsupported initial value type: {attributeName}"),
                };

                try
                {
                    // Process the initial value based on the contract parameter type
                    switch (parameterType)
                    {
                        case ContractParameterType.String:
                            Push(value);
                            break;
                        case ContractParameterType.ByteArray:
                            Push(value.HexToBytes(true));
                            break;
                        case ContractParameterType.Hash160:
                            Push((UInt160.TryParse(value, out var hash) ? hash : value.ToScriptHash(_context.Options.AddressVersion)).ToArray());
                            break;
                        case ContractParameterType.PublicKey:
                            Push(ECPoint.Parse(value, ECCurve.Secp256r1).EncodePoint(true));
                            break;
                        default:
                            throw new CompilationException(field, DiagnosticId.InvalidInitialValueType, $"Unsupported initial value type: {parameterType}");
                    }
                }
                catch (Exception ex) when (ex is not CompilationException)
                {
                    // If an exception occurs during the processing of the initial value (excluding CompilationException),
                    // throw a CompilationException with the field information and the invalid initial value details
                    throw new CompilationException(field, DiagnosticId.InvalidInitialValue, $"Invalid initial value: {value} of type: {parameterType}");
                }

                // Invoke the post-initialization action, if provided
                postInitialize?.Invoke();
            }
        }
        private IEnumerable<(byte fieldIndex, AttributeData attribute)> ConvertModifier(SemanticModel model)
        {
            foreach (var attribute in Symbol.GetAttributesWithInherited())
            {
                if (attribute.AttributeClass?.IsSubclassOf(nameof(ModifierAttribute)) != true)
                    continue;

                JumpTarget notNullTarget = new();
                byte fieldIndex = _context.AddAnonymousStaticField();
                AccessSlot(OpCode.LDSFLD, fieldIndex);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIFNOT_L, notNullTarget);

                MethodConvert constructor = _context.ConvertMethod(model, attribute.AttributeConstructor!);
                CreateObject(model, attribute.AttributeClass, null);
                foreach (var arg in attribute.ConstructorArguments.Reverse())
                    Push(arg.Value);
                Push(attribute.ConstructorArguments.Length);
                AddInstruction(OpCode.PICK);
                EmitCall(constructor);
                AccessSlot(OpCode.STSFLD, fieldIndex);

                notNullTarget.Instruction = AccessSlot(OpCode.LDSFLD, fieldIndex);
                var enterSymbol = attribute.AttributeClass.GetAllMembers()
                    .OfType<IMethodSymbol>()
                    .First(p => p.Name == nameof(ModifierAttribute.Enter) && p.Parameters.Length == 0);
                MethodConvert enterMethod = _context.ConvertMethod(model, enterSymbol);
                EmitCall(enterMethod);
                yield return (fieldIndex, attribute);
            }
        }

        private Instruction? ExitModifier(SemanticModel model, byte fieldIndex, AttributeData attribute)
        {
            var exitSymbol = attribute.AttributeClass!.GetAllMembers()
                .OfType<IMethodSymbol>()
                .First(p => p.Name == nameof(ModifierAttribute.Exit) && p.Parameters.Length == 0);
            MethodConvert exitMethod = _context.ConvertMethod(model, exitSymbol);
            if (exitMethod.IsEmpty) return null;
            var instruction = AccessSlot(OpCode.LDSFLD, fieldIndex);
            EmitCall(exitMethod);
            return instruction;
        }
        #endregion

        #region Helper

        /// <summary>
        /// load parameter value
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private Instruction LdArgSlot(IParameterSymbol parameter)
        {
            if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
            {
                //using created static fields
                return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
            }
            if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter))
            {
                //create static fields from captrued parameter
                var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
                CapturedLocalSymbols.Add(parameter);
                return AccessSlot(OpCode.LDSFLD, staticIndex);
            }
            // local parameter in current method
            byte index = _parameters[parameter];
            return AccessSlot(OpCode.LDARG, index);
        }

        /// <summary>
        /// store value to parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private Instruction StArgSlot(IParameterSymbol parameter)
        {
            if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
            {
                //using created static fields
                return AccessSlot(OpCode.STSFLD, staticFieldIndex);
            }
            if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter))
            {
                //create static fields from captrued parameter
                var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
                CapturedLocalSymbols.Add(parameter);
                return AccessSlot(OpCode.STSFLD, staticIndex);
            }
            // local parameter in current method
            byte index = _parameters[parameter];
            return AccessSlot(OpCode.STARG, index);
        }

        /// <summary>
        /// load local variable value
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        private Instruction LdLocSlot(ILocalSymbol local)
        {
            if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
            {
                //using created static fields
                return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
            }
            if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local))
            {
                //create static fields from captrued local
                byte staticIndex = _context.GetOrAddCapturedStaticField(local);
                CapturedLocalSymbols.Add(local);
                return AccessSlot(OpCode.LDSFLD, staticIndex);
            }
            // local variables in current method
            byte index = _localVariables[local];
            return AccessSlot(OpCode.LDLOC, index);
        }

        /// <summary>
        /// store value to local variable
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        private Instruction StLocSlot(ILocalSymbol local)
        {
            if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
            {
                //using created static fields
                return AccessSlot(OpCode.STSFLD, staticFieldIndex);
            }
            if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local))
            {
                //create static fields from captrued local
                byte staticIndex = _context.GetOrAddCapturedStaticField(local);
                CapturedLocalSymbols.Add(local);
                return AccessSlot(OpCode.STSFLD, staticIndex);
            }
            byte index = _localVariables[local];
            return AccessSlot(OpCode.STLOC, index);
        }

        private Instruction AccessSlot(OpCode opcode, byte index)
        {
            return index >= 7
                ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
                : AddInstruction(opcode - 7 + index);
        }

        private bool TryProcessInlineMethods(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments)
        {
            SyntaxNode? syntaxNode = null;
            if (!symbol.DeclaringSyntaxReferences.IsEmpty)
                syntaxNode = symbol.DeclaringSyntaxReferences[0].GetSyntax();

            if (syntaxNode is not BaseMethodDeclarationSyntax syntax) return false;
            if (!symbol.GetAttributesWithInherited().Any(attribute => attribute.ConstructorArguments.Length > 0
                    && attribute.AttributeClass?.Name == nameof(MethodImplAttribute)
                    && attribute.ConstructorArguments[0].Value is not null
                    && (MethodImplOptions)attribute.ConstructorArguments[0].Value! == MethodImplOptions.AggressiveInlining))
                return false;

            _internalInline = true;

            using (InsertSequencePoint(syntax))
            {
                if (arguments is not null) PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
                if (syntax.Body != null) ConvertStatement(model, syntax.Body);
            }
            return true;
        }

        private void PrepareArgumentsForMethod(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            var namedArguments = arguments.OfType<ArgumentSyntax>().Where(p => p.NameColon is not null).Select(p => (Symbol: (IParameterSymbol)model.GetSymbolInfo(p.NameColon!.Name).Symbol!, p.Expression)).ToDictionary(p => p.Symbol, p => p.Expression, (IEqualityComparer<IParameterSymbol>)SymbolEqualityComparer.Default);
            IEnumerable<IParameterSymbol> parameters = symbol.Parameters;
            if (callingConvention == CallingConvention.Cdecl)
                parameters = parameters.Reverse();
            foreach (IParameterSymbol parameter in parameters)
            {
                if (namedArguments.TryGetValue(parameter, out ExpressionSyntax? expression))
                {
                    ConvertExpression(model, expression);
                }
                else if (parameter.IsParams)
                {
                    if (arguments.Count > parameter.Ordinal)
                    {
                        if (arguments.Count == parameter.Ordinal + 1)
                        {
                            expression = arguments[parameter.Ordinal] switch
                            {
                                ArgumentSyntax argument => argument.Expression,
                                ExpressionSyntax exp => exp,
                                _ => throw new CompilationException(arguments[parameter.Ordinal], DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {arguments[parameter.Ordinal]}"),
                            };
                            Conversion conversion = model.ClassifyConversion(expression, parameter.Type);
                            if (conversion.Exists)
                            {
                                ConvertExpression(model, expression);
                                continue;
                            }
                        }
                        for (int i = arguments.Count - 1; i >= parameter.Ordinal; i--)
                        {
                            expression = arguments[i] switch
                            {
                                ArgumentSyntax argument => argument.Expression,
                                ExpressionSyntax exp => exp,
                                _ => throw new CompilationException(arguments[i], DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {arguments[i]}"),
                            };
                            ConvertExpression(model, expression);
                        }
                        Push(arguments.Count - parameter.Ordinal);
                        AddInstruction(OpCode.PACK);
                    }
                    else
                    {
                        AddInstruction(OpCode.NEWARRAY0);
                    }
                }
                else
                {
                    if (arguments.Count > parameter.Ordinal)
                    {
                        switch (arguments[parameter.Ordinal])
                        {
                            case ArgumentSyntax argument:
                                if (argument.NameColon is null)
                                {
                                    ConvertExpression(model, argument.Expression);
                                    continue;
                                }
                                break;
                            case ExpressionSyntax ex:
                                ConvertExpression(model, ex);
                                continue;
                            default:
                                throw new CompilationException(arguments[parameter.Ordinal], DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {arguments[parameter.Ordinal]}");
                        }
                    }
                    Push(parameter.ExplicitDefaultValue);
                }
            }
        }

        private Instruction IsType(VM.Types.StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.ISTYPE,
                Operand = new[] { (byte)type }
            });
        }

        private Instruction ChangeType(VM.Types.StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.CONVERT,
                Operand = new[] { (byte)type }
            });
        }

        private void InitializeFieldForObject(SemanticModel model, IFieldSymbol field, InitializerExpressionSyntax? initializer)
        {
            ExpressionSyntax? expression = null;
            if (initializer is not null)
            {
                foreach (ExpressionSyntax e in initializer.Expressions)
                {
                    if (e is not AssignmentExpressionSyntax ae)
                        throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {initializer}");
                    if (SymbolEqualityComparer.Default.Equals(field, model.GetSymbolInfo(ae.Left).Symbol))
                    {
                        expression = ae.Right;
                        break;
                    }
                }
            }
            if (expression is null)
                PushDefault(field.Type);
            else
                ConvertExpression(model, expression);
        }

        /// <summary>
        /// Creates a new object of the specified type (class/struct) and initializes its fields.
        /// </summary>
        /// <param name="model">The semantic model used for compilation.</param>
        /// <param name="type">The type of the object to be created.</param>
        /// <param name="initializer">The initializer expression syntax for the object, if any.</param>
        /// <remarks>
        /// This method creates a new object of the specified <paramref name="type"/> and initializes its fields.
        /// The process of creating and initializing the object depends on whether the type is a value type or a reference type,
        /// and whether it has any fields or virtual methods.
        ///
        /// If the type is a value type or has no fields, the method takes the following steps:
        ///     1. Adds an <see cref="OpCode.NEWSTRUCT0"/> instruction for value types, or a <see cref="OpCode.NEWARRAY0"/> instruction for reference types with no fields.
        ///     2. For each field in the type, it duplicates the object reference (using <see cref="OpCode.DUP"/>),
        ///         initializes the field value (using <see cref="InitializeFieldForObject"/>), and appends the field to the object (using <see cref="OpCode.APPEND"/>).
        ///
        /// If the type is a reference type with fields, the method takes the following steps:
        ///     1. Initializes each field in reverse order by calling <see cref="InitializeFieldForObject"/> for each field.
        ///     2. Pushes the number of fields onto the evaluation stack.
        ///     3. Adds an <see cref="OpCode.PACK"/> instruction to create an array representing the object's fields.
        ///
        /// If the type has any virtual methods, the method adds the object's virtual method table to the object:
        ///     1. Retrieves the index of the virtual method table for the type using <see cref="CompilationContext.AddVTable"/>.
        ///     2. Duplicates the object reference (using <see cref="OpCode.DUP"/>).
        ///     3. Loads the virtual method table onto the evaluation stack (using <see cref="AccessSlot"/> with <see cref="OpCode.LDSFLD"/>).
        ///     4. Appends the virtual method table to the object (using <see cref="OpCode.APPEND"/>).
        ///
        /// The <paramref name="initializer"/> parameter is used to pass any initializer expressions for the object's fields,
        /// which are handled by the <see cref="InitializeFieldForObject"/> method.
        /// </remarks>
        private void CreateObject(SemanticModel model, ITypeSymbol type, InitializerExpressionSyntax? initializer)
        {
            // Get all non-static members of the type
            //
            ISymbol[] members = type.GetAllMembers().Where(p => !p.IsStatic).ToArray();

            // Get all field symbols from the non-static members
            IFieldSymbol[] fields = members.OfType<IFieldSymbol>().ToArray();

            if (fields.Length == 0 || type.IsValueType)
            {
                // If the type is a value type or has no fields

                // Add NEWSTRUCT0 instruction for value types, or NEWARRAY0 instruction for reference types with no fields
                AddInstruction(type.IsValueType ? OpCode.NEWSTRUCT0 : OpCode.NEWARRAY0);

                foreach (IFieldSymbol field in fields)
                {
                    // For each field in the type

                    // Duplicate the object reference on the stack
                    AddInstruction(OpCode.DUP);

                    // Initialize the field value
                    InitializeFieldForObject(model, field, initializer);

                    // Append the field to the object
                    AddInstruction(OpCode.APPEND);
                }
            }
            else
            {
                // If the type is a reference type with fields

                // Initialize each field in reverse order
                for (int i = fields.Length - 1; i >= 0; i--)
                {
                    InitializeFieldForObject(model, fields[i], initializer);
                }

                // Push the number of fields onto the evaluation stack
                Push(fields.Length);

                // Create an array representing the object's fields
                AddInstruction(OpCode.PACK);
            }

            // Get all virtual method symbols from the non-static members
            IMethodSymbol[] virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();

            if (virtualMethods.Length > 0)
            {
                // If the type has any virtual methods

                // Get the index of the virtual method table for the type
                byte index = _context.AddVTable(type);

                // Duplicate the object reference on the stack
                AddInstruction(OpCode.DUP);

                // Load the virtual method table onto the evaluation stack
                AccessSlot(OpCode.LDSFLD, index);

                // Append the virtual method table to the object
                AddInstruction(OpCode.APPEND);
            }
        }

        private Instruction Jump(OpCode opcode, JumpTarget target)
        {
            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Target = target
            });
        }

        /// <summary>
        /// Convert a throw expression or throw statement to OpCodes.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the Throw.</param>
        /// <param name="exception">The content of exception</param>
        /// <exception cref="CompilationException">Only a single parameter is supported for exceptions.</exception>
        /// <example>
        /// throw statement:
        /// <code>
        /// if (shapeAmount <= 0)
        /// {
        ///     throw new Exception("Amount of shapes must be positive.");
        /// }
        ///</code>
        /// throw expression:
        /// <code>
        /// string a = null;
        /// var b = a ?? throw new Exception();
        /// </code>
        /// <code>
        /// var first = args.Length >= 1 ? args[0] : throw new Exception();
        /// </code>
        /// </example>
        /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-throw-expression">The throw expression</seealso>
        /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-catch-statement">Exception-handling statements - throw</seealso>
        private void Throw(SemanticModel model, ExpressionSyntax? exception)
        {
            if (exception is not null)
            {
                ITypeSymbol type = model.GetTypeInfo(exception).Type!;
                if (type.IsSubclassOf(nameof(scfx::Neo.SmartContract.Framework.UncatchableException), includeThisClass: true))
                {
                    AddInstruction(OpCode.ABORT);
                    return;
                }
            }
            switch (exception)
            {
                case ObjectCreationExpressionSyntax expression:
                    switch (expression.ArgumentList?.Arguments.Count)
                    {
                        case null:
                        case 0:
                            Push("exception");
                            break;
                        case 1:
                            ConvertExpression(model, expression.ArgumentList.Arguments[0].Expression);
                            break;
                        default:
                            throw new CompilationException(expression, DiagnosticId.MultiplyThrows, "Only a single parameter is supported for exceptions.");
                    }
                    break;
                case null:
                    AccessSlot(OpCode.LDLOC, _exceptionStack.Peek());
                    break;
                default:
                    ConvertExpression(model, exception);
                    break;
            }
            AddInstruction(OpCode.THROW);
        }
        #endregion
    }

    class MethodConvertCollection : KeyedCollection<IMethodSymbol, MethodConvert>
    {
        protected override IMethodSymbol GetKeyForItem(MethodConvert item) => item.Symbol;
    }
}
