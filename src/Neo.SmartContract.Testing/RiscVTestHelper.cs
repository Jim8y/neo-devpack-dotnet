// Copyright (C) 2015-2026 The Neo Project.
//
// RiscVTestHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Helper class for building and testing RISC-V contracts.
    /// </summary>
    public static class RiscVTestHelper
    {
        public const string CargoEnvironmentVariable = "CARGO";
        public const string RustcEnvironmentVariable = "RUSTC";
        public const string NeoRiscvGuestPathEnvironmentVariable = "NEO_RISCV_GUEST_PATH";

        /// <summary>
        /// Checks if the Rust toolchain is available for building RISC-V contracts.
        /// </summary>
        public static bool IsRustToolchainAvailable()
        {
            return !string.IsNullOrEmpty(GetCargoPath()) &&
                   !string.IsNullOrEmpty(GetRustcPath());
        }

        /// <summary>
        /// Gets the path to the cargo executable.
        /// </summary>
        public static string? GetCargoPath()
        {
            var path = Environment.GetEnvironmentVariable(CargoEnvironmentVariable);
            if (!string.IsNullOrEmpty(path))
                return path;

            // Try to find cargo in PATH
            try
            {
                using var process = new Process();
                process.StartInfo.FileName = "which";
                process.StartInfo.Arguments = "cargo";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                var output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                return process.ExitCode == 0 ? output : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the path to the rustc executable.
        /// </summary>
        public static string? GetRustcPath()
        {
            var path = Environment.GetEnvironmentVariable(RustcEnvironmentVariable);
            if (!string.IsNullOrEmpty(path))
                return path;

            // Try to find rustc in PATH
            try
            {
                using var process = new Process();
                process.StartInfo.FileName = "which";
                process.StartInfo.Arguments = "rustc";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                var output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                return process.ExitCode == 0 ? output : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Builds a RISC-V contract from Rust source code.
        /// </summary>
        /// <param name="sourcePath">Path to the Rust source directory containing Cargo.toml.</param>
        /// <param name="outputPath">Optional output path for the .polkavm binary.</param>
        /// <returns>The path to the built .polkavm binary.</returns>
        public static string BuildContract(string sourcePath, string? outputPath = null)
        {
            if (!IsRustToolchainAvailable())
                throw new InvalidOperationException("Rust toolchain is not available.");

            if (!Directory.Exists(sourcePath))
                throw new DirectoryNotFoundException($"Source directory not found: {sourcePath}");

            var cargoToml = Path.Combine(sourcePath, "Cargo.toml");
            if (!File.Exists(cargoToml))
                throw new FileNotFoundException($"Cargo.toml not found in {sourcePath}");

            // Determine target directory
            var targetDir = Path.Combine(sourcePath, "target");
            var profile = "release";

            // Build the contract
            var cargoPath = GetCargoPath()!;
            using var process = new Process();
            process.StartInfo.FileName = cargoPath;
            process.StartInfo.Arguments = $"build --release --target-dir {targetDir}";
            process.StartInfo.WorkingDirectory = sourcePath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Cargo build failed:\n{stderr}\n{stdout}");
            }

            // Find the output binary
            var binaryName = GetBinaryNameFromCargoToml(cargoToml);
            var binaryPath = Path.Combine(targetDir, profile, $"{binaryName}.polkavm");

            if (!File.Exists(binaryPath))
            {
                // Try alternative naming
                binaryPath = Path.Combine(targetDir, profile, binaryName, "guest.polkavm");
            }

            if (!File.Exists(binaryPath))
            {
                throw new FileNotFoundException($"Built binary not found at expected path: {binaryPath}");
            }

            // Copy to output path if specified
            if (!string.IsNullOrEmpty(outputPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                File.Copy(binaryPath, outputPath, overwrite: true);
                return outputPath;
            }

            return binaryPath;
        }

        /// <summary>
        /// Builds a RISC-V contract and returns the binary data.
        /// </summary>
        /// <param name="sourcePath">Path to the Rust source directory.</param>
        /// <returns>The binary data of the built .polkavm contract.</returns>
        public static byte[] BuildContractAndGetBytes(string sourcePath)
        {
            var binaryPath = BuildContract(sourcePath);
            return File.ReadAllBytes(binaryPath);
        }

        /// <summary>
        /// Creates a minimal test contract for testing purposes.
        /// </summary>
        /// <param name="outputDirectory">Directory to create the test contract in.</param>
        /// <returns>Path to the created contract directory.</returns>
        public static string CreateMinimalTestContract(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);

            // Create Cargo.toml
            var cargoToml = @"[package]
name = ""test_contract""
version = ""0.1.0""
edition = ""2021""

[dependencies]
neo-riscv-guest = { path = """" }

[profile.release]
opt-level = 3
lto = true
"";

            // Try to find the neo-riscv-guest path
            var guestPath = Environment.GetEnvironmentVariable(NeoRiscvGuestPathEnvironmentVariable);
            if (string.IsNullOrEmpty(guestPath))
            {
                // Try to find it relative to the current directory
                var possiblePaths = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "crates", "neo-riscv-guest"),
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "crates", "neo-riscv-guest"),
                    Path.Combine(Directory.GetCurrentDirectory(), "crates", "neo-riscv-guest"),
                };

                foreach (var path in possiblePaths)
                {
                    if (Directory.Exists(path))
                    {
                        guestPath = Path.GetFullPath(path);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(guestPath))
            {
                cargoToml = cargoToml.Replace("\"\"", $"\"{guestPath.Replace("\\", "/")}\"");
            }
            else
            {
                cargoToml = cargoToml.Replace("{ path = \"\"\"" }", "");
            }

            File.WriteAllText(Path.Combine(outputDirectory, "Cargo.toml"), cargoToml);

            // Create src directory and main.rs
            var srcDir = Path.Combine(outputDirectory, "src");
            Directory.CreateDirectory(srcDir);

            var mainRs = @"#![no_std]
#![no_main]

use neo_riscv_guest::prelude::*;

#[no_mangle]
pub extern ""C"" fn main() {
    // Minimal contract that just returns
}

#[no_mangle]
pub extern ""C"" fn add(a: i64, b: i64) -> i64 {
    a + b
}

#[panic_handler]
fn panic(_info: &core::panic::PanicInfo) -> ! {
    loop {}
}
"";

            File.WriteAllText(Path.Combine(srcDir, "main.rs"), mainRs);

            return outputDirectory;
        }

        /// <summary>
        /// Creates stack items for RISC-V contract invocation.
        /// </summary>
        public static class StackItems
        {
            public static Integer Integer(long value) => new(value);
            public static Integer Integer(BigInteger value) => new(value);
            public static ByteString ByteString(byte[] value) => new(value);
            public static ByteString ByteString(string value) => new(Encoding.UTF8.GetBytes(value));
            public static Neo.VM.Types.Boolean Boolean(bool value) => value ? StackItem.True : StackItem.False;
            public static Neo.VM.Types.Null Null() => StackItem.Null;

            public static Neo.VM.Types.Array Array(params StackItem[] items)
            {
                var array = new Neo.VM.Types.Array();
                foreach (var item in items)
                    array.Add(item);
                return array;
            }
        }

        /// <summary>
        /// Converts a hex string to bytes.
        /// </summary>
        public static byte[] HexToBytes(string hex)
        {
            hex = hex.Replace(" ", "").Replace("0x", "");
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have even length.");

            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// Converts bytes to a hex string.
        /// </summary>
        public static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        private static string GetBinaryNameFromCargoToml(string cargoTomlPath)
        {
            var content = File.ReadAllText(cargoTomlPath);
            var lines = content.Split('\n');

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("name = "))
                {
                    var value = trimmed.Substring(7).Trim().Trim('"');
                    return value;
                }
            }

            return "test_contract";
        }
    }
}
