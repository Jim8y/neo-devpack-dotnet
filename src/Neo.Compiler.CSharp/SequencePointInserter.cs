// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Neo.Compiler
{
    /// <summary>
    /// Represents a utility class for inserting sequence points into a list of instructions.
    /// <remarks>
    /// In C#, a SequencePoint represents a point in the code where the debugger can safely stop and inspect the program state.
    /// It is a concept related to debugging and the mapping between source code and the compiled executable.
    /// Sequence points are used by the debugger to determine valid locations where breakpoints can be set
    /// and where the program can be paused for inspection. They are also used to ensure that the program state is
    /// consistent and matches the expected state based on the source code.
    /// </remarks>
    /// </summary>
    class SequencePointInserter : IDisposable
    {
        private readonly IReadOnlyList<Instruction> instructions;
        private readonly Location? location;
        private readonly int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequencePointInserter"/> class.
        /// </summary>
        /// <param name="instructions">The list of instructions.</param>
        /// <param name="syntax">The syntax node or token.</param>
        public SequencePointInserter(IReadOnlyList<Instruction> instructions, SyntaxNodeOrToken? syntax) :
            this(instructions, syntax?.GetLocation())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequencePointInserter"/> class.
        /// </summary>
        /// <param name="instructions">The list of instructions.</param>
        /// <param name="syntax">The syntax reference.</param>
        public SequencePointInserter(IReadOnlyList<Instruction> instructions, SyntaxReference? syntax) :
           this(instructions, syntax?.SyntaxTree.GetLocation(syntax.Span))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequencePointInserter"/> class.
        /// </summary>
        /// <param name="instructions">The list of instructions.</param>
        /// <param name="location">The location.</param>
        public SequencePointInserter(IReadOnlyList<Instruction> instructions, Location? location)
        {
            this.instructions = instructions;
            this.location = location;
            this.position = instructions.Count;

            // No location must be removed

            if (this.location?.SourceTree is null)
                this.location = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (location == null) return;

            for (int x = position; x < instructions.Count; x++)
            {
                if (instructions[x].SourceLocation is null)
                {
                    instructions[x].SourceLocation = location;
                }
            }
        }
    }
}
