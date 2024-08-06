using Akka.Util.Internal;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

internal static class Helper
{
    internal static void TestCodeBlock(string codeBlock)
    {
        var result = new CompilationEngine(new CompilationOptions()
        {
            Debug = true,
            CompilerVersion = "TestingEngine",
            Optimize = CompilationOptions.OptimizationType.All,
        }).CompileFromCodeBlock(codeBlock).First();
        if (result.Success) return;

        result.Diagnostics.ForEach(Console.WriteLine);
        const string redColor = "\u001b[31m";
        const string resetColor = "\u001b[0m";
        Console.WriteLine($"{redColor}Error compiling code block : {{\n\t{codeBlock.Replace("\n", "\n\t")}\n}}{resetColor}");
    }
}
