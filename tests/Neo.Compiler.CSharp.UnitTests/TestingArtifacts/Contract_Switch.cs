using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Switch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Switch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""switchLong"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""switch6"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":242,""safe"":false},{""name"":""switch6Inline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":306,""safe"":false},{""name"":""switchInteger"",""parameters"":[{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":373,""safe"":false},{""name"":""switchLongLong"",""parameters"":[{""name"":""test"",""type"":""String""}],""returntype"":""Any"",""offset"":413,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1pA1cBAXhwaAwBMJcltgAAAGgMATGXJa4AAABoDAEylyWmAAAAaAwBM5clngAAAGgMATSXJZYAAABoDAE1lyWOAAAAaAwBNpclhgAAAGgMATeXJX4AAABoDAE4lyV2AAAAaAwBOZckbmgMAjEwlyRoaAwCMTGXJGJoDAIxMpckXGgMAjEzlyRWaAwCMTSXJFBoDAIxNZckSmgMAjE2lyREaAwCMTeXJD9oDAIxOJckOmgMAjE5lyQ1aAwCMjCXJDAiMRFAEkATQBRAFUAWQBdAGEAZQBpAG0AcQB1AHkAfQCBAABFAABJAABNAABRAABVAAGNAVwEBeHBoDAEwlyQnaAwBMZckImgMATKXJB1oDAEzlyQYaAwBNJckE2gMATWXJA4iDhFAEkATQBRAFUAWQABjQFcBAXhwaAwBMJcmBBFAaAwBMZcmBBJAaAwBMpcmBBNAaAwBM5cmBBRAaAwBNJcmBBVAaAwBNZcmBBZACCYFAGNAaDpXAgERcHhxaRGXJA5pEpckDWkTlyQMIg4ScCIME3AiCBZwIgQQcGhAVwIBEXB4cWkMAWGXJEBpDAFjlyRxaQwBYpcloQAAAGkMAWSXJc8AAABpDAFllyX8AAAAaQwBZpclJgEAAGkMAWeXJVABAAAjfwEAAGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSNLAQAAaBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AjFAEAAGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSPcAAAAaA+gSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AjpQAAAGhooEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wIm5oE6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCI6aBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AiBmgRoXBoQLFgO0o="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJCdoDDGXJCJoDDKXJB1oDDOXJBhoDDSXJBNoDDWXJA4iDhFAEkATQBRAFUAWQABjQA==
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.PUSHDATA1 30 	-> 8 datoshi
    /// 09 : OpCode.EQUAL 	-> 32 datoshi
    /// 0A : OpCode.JMPIF 27 	-> 2 datoshi
    /// 0C : OpCode.LDLOC0 	-> 2 datoshi
    /// 0D : OpCode.PUSHDATA1 31 	-> 8 datoshi
    /// 10 : OpCode.EQUAL 	-> 32 datoshi
    /// 11 : OpCode.JMPIF 22 	-> 2 datoshi
    /// 13 : OpCode.LDLOC0 	-> 2 datoshi
    /// 14 : OpCode.PUSHDATA1 32 	-> 8 datoshi
    /// 17 : OpCode.EQUAL 	-> 32 datoshi
    /// 18 : OpCode.JMPIF 1D 	-> 2 datoshi
    /// 1A : OpCode.LDLOC0 	-> 2 datoshi
    /// 1B : OpCode.PUSHDATA1 33 	-> 8 datoshi
    /// 1E : OpCode.EQUAL 	-> 32 datoshi
    /// 1F : OpCode.JMPIF 18 	-> 2 datoshi
    /// 21 : OpCode.LDLOC0 	-> 2 datoshi
    /// 22 : OpCode.PUSHDATA1 34 	-> 8 datoshi
    /// 25 : OpCode.EQUAL 	-> 32 datoshi
    /// 26 : OpCode.JMPIF 13 	-> 2 datoshi
    /// 28 : OpCode.LDLOC0 	-> 2 datoshi
    /// 29 : OpCode.PUSHDATA1 35 	-> 8 datoshi
    /// 2C : OpCode.EQUAL 	-> 32 datoshi
    /// 2D : OpCode.JMPIF 0E 	-> 2 datoshi
    /// 2F : OpCode.JMP 0E 	-> 2 datoshi
    /// 31 : OpCode.PUSH1 	-> 1 datoshi
    /// 32 : OpCode.RET 	-> 0 datoshi
    /// 33 : OpCode.PUSH2 	-> 1 datoshi
    /// 34 : OpCode.RET 	-> 0 datoshi
    /// 35 : OpCode.PUSH3 	-> 1 datoshi
    /// 36 : OpCode.RET 	-> 0 datoshi
    /// 37 : OpCode.PUSH4 	-> 1 datoshi
    /// 38 : OpCode.RET 	-> 0 datoshi
    /// 39 : OpCode.PUSH5 	-> 1 datoshi
    /// 3A : OpCode.RET 	-> 0 datoshi
    /// 3B : OpCode.PUSH6 	-> 1 datoshi
    /// 3C : OpCode.RET 	-> 0 datoshi
    /// 3D : OpCode.PUSHINT8 63 	-> 1 datoshi
    /// 3F : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("switch6")]
    public abstract object? Switch6(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJgQRQGgMMZcmBBJAaAwylyYEE0BoDDOXJgQUQGgMNJcmBBVAaAw1lyYEFkAIJgUAY0BoOg==
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.PUSHDATA1 30 	-> 8 datoshi
    /// 09 : OpCode.EQUAL 	-> 32 datoshi
    /// 0A : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 0C : OpCode.PUSH1 	-> 1 datoshi
    /// 0D : OpCode.RET 	-> 0 datoshi
    /// 0E : OpCode.LDLOC0 	-> 2 datoshi
    /// 0F : OpCode.PUSHDATA1 31 	-> 8 datoshi
    /// 12 : OpCode.EQUAL 	-> 32 datoshi
    /// 13 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 15 : OpCode.PUSH2 	-> 1 datoshi
    /// 16 : OpCode.RET 	-> 0 datoshi
    /// 17 : OpCode.LDLOC0 	-> 2 datoshi
    /// 18 : OpCode.PUSHDATA1 32 	-> 8 datoshi
    /// 1B : OpCode.EQUAL 	-> 32 datoshi
    /// 1C : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 1E : OpCode.PUSH3 	-> 1 datoshi
    /// 1F : OpCode.RET 	-> 0 datoshi
    /// 20 : OpCode.LDLOC0 	-> 2 datoshi
    /// 21 : OpCode.PUSHDATA1 33 	-> 8 datoshi
    /// 24 : OpCode.EQUAL 	-> 32 datoshi
    /// 25 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 27 : OpCode.PUSH4 	-> 1 datoshi
    /// 28 : OpCode.RET 	-> 0 datoshi
    /// 29 : OpCode.LDLOC0 	-> 2 datoshi
    /// 2A : OpCode.PUSHDATA1 34 	-> 8 datoshi
    /// 2D : OpCode.EQUAL 	-> 32 datoshi
    /// 2E : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 30 : OpCode.PUSH5 	-> 1 datoshi
    /// 31 : OpCode.RET 	-> 0 datoshi
    /// 32 : OpCode.LDLOC0 	-> 2 datoshi
    /// 33 : OpCode.PUSHDATA1 35 	-> 8 datoshi
    /// 36 : OpCode.EQUAL 	-> 32 datoshi
    /// 37 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 39 : OpCode.PUSH6 	-> 1 datoshi
    /// 3A : OpCode.RET 	-> 0 datoshi
    /// 3B : OpCode.PUSHT 	-> 1 datoshi
    /// 3C : OpCode.JMPIFNOT 05 	-> 2 datoshi
    /// 3E : OpCode.PUSHINT8 63 	-> 1 datoshi
    /// 40 : OpCode.RET 	-> 0 datoshi
    /// 41 : OpCode.LDLOC0 	-> 2 datoshi
    /// 42 : OpCode.THROW 	-> 512 datoshi
    /// </remarks>
    [DisplayName("switch6Inline")]
    public abstract object? Switch6Inline(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEXB4cWkRlyQOaRKXJA1pE5ckDCIOEnAiDBNwIggWcCIEEHBoQA==
    /// 00 : OpCode.INITSLOT 0201 	-> 64 datoshi
    /// 03 : OpCode.PUSH1 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDARG0 	-> 2 datoshi
    /// 06 : OpCode.STLOC1 	-> 2 datoshi
    /// 07 : OpCode.LDLOC1 	-> 2 datoshi
    /// 08 : OpCode.PUSH1 	-> 1 datoshi
    /// 09 : OpCode.EQUAL 	-> 32 datoshi
    /// 0A : OpCode.JMPIF 0E 	-> 2 datoshi
    /// 0C : OpCode.LDLOC1 	-> 2 datoshi
    /// 0D : OpCode.PUSH2 	-> 1 datoshi
    /// 0E : OpCode.EQUAL 	-> 32 datoshi
    /// 0F : OpCode.JMPIF 0D 	-> 2 datoshi
    /// 11 : OpCode.LDLOC1 	-> 2 datoshi
    /// 12 : OpCode.PUSH3 	-> 1 datoshi
    /// 13 : OpCode.EQUAL 	-> 32 datoshi
    /// 14 : OpCode.JMPIF 0C 	-> 2 datoshi
    /// 16 : OpCode.JMP 0E 	-> 2 datoshi
    /// 18 : OpCode.PUSH2 	-> 1 datoshi
    /// 19 : OpCode.STLOC0 	-> 2 datoshi
    /// 1A : OpCode.JMP 0C 	-> 2 datoshi
    /// 1C : OpCode.PUSH3 	-> 1 datoshi
    /// 1D : OpCode.STLOC0 	-> 2 datoshi
    /// 1E : OpCode.JMP 08 	-> 2 datoshi
    /// 20 : OpCode.PUSH6 	-> 1 datoshi
    /// 21 : OpCode.STLOC0 	-> 2 datoshi
    /// 22 : OpCode.JMP 04 	-> 2 datoshi
    /// 24 : OpCode.PUSH0 	-> 1 datoshi
    /// 25 : OpCode.STLOC0 	-> 2 datoshi
    /// 26 : OpCode.LDLOC0 	-> 2 datoshi
    /// 27 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("switchInteger")]
    public abstract object? SwitchInteger(BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJbYAAABoDDGXJa4AAABoDDKXJaYAAABoDDOXJZ4AAABoDDSXJZYAAABoDDWXJY4AAABoDDaXJYYAAABoDDeXJX4AAABoDDiXJXYAAABoDDmXJG5oDDEwlyRoaAwxMZckYmgMMTKXJFxoDDEzlyRWaAwxNJckUGgMMTWXJEpoDDE2lyREaAwxN5ckP2gMMTiXJDpoDDE5lyQ1aAwyMJckMCIxEUASQBNAFEAVQBZAF0AYQBlAGkAbQBxAHUAeQB9AIEAAEUAAEkAAE0AAFEAAFUAAY0A=
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.PUSHDATA1 30 	-> 8 datoshi
    /// 09 : OpCode.EQUAL 	-> 32 datoshi
    /// 0A : OpCode.JMPIF_L B6000000 	-> 2 datoshi
    /// 0F : OpCode.LDLOC0 	-> 2 datoshi
    /// 10 : OpCode.PUSHDATA1 31 	-> 8 datoshi
    /// 13 : OpCode.EQUAL 	-> 32 datoshi
    /// 14 : OpCode.JMPIF_L AE000000 	-> 2 datoshi
    /// 19 : OpCode.LDLOC0 	-> 2 datoshi
    /// 1A : OpCode.PUSHDATA1 32 	-> 8 datoshi
    /// 1D : OpCode.EQUAL 	-> 32 datoshi
    /// 1E : OpCode.JMPIF_L A6000000 	-> 2 datoshi
    /// 23 : OpCode.LDLOC0 	-> 2 datoshi
    /// 24 : OpCode.PUSHDATA1 33 	-> 8 datoshi
    /// 27 : OpCode.EQUAL 	-> 32 datoshi
    /// 28 : OpCode.JMPIF_L 9E000000 	-> 2 datoshi
    /// 2D : OpCode.LDLOC0 	-> 2 datoshi
    /// 2E : OpCode.PUSHDATA1 34 	-> 8 datoshi
    /// 31 : OpCode.EQUAL 	-> 32 datoshi
    /// 32 : OpCode.JMPIF_L 96000000 	-> 2 datoshi
    /// 37 : OpCode.LDLOC0 	-> 2 datoshi
    /// 38 : OpCode.PUSHDATA1 35 	-> 8 datoshi
    /// 3B : OpCode.EQUAL 	-> 32 datoshi
    /// 3C : OpCode.JMPIF_L 8E000000 	-> 2 datoshi
    /// 41 : OpCode.LDLOC0 	-> 2 datoshi
    /// 42 : OpCode.PUSHDATA1 36 	-> 8 datoshi
    /// 45 : OpCode.EQUAL 	-> 32 datoshi
    /// 46 : OpCode.JMPIF_L 86000000 	-> 2 datoshi
    /// 4B : OpCode.LDLOC0 	-> 2 datoshi
    /// 4C : OpCode.PUSHDATA1 37 	-> 8 datoshi
    /// 4F : OpCode.EQUAL 	-> 32 datoshi
    /// 50 : OpCode.JMPIF_L 7E000000 	-> 2 datoshi
    /// 55 : OpCode.LDLOC0 	-> 2 datoshi
    /// 56 : OpCode.PUSHDATA1 38 	-> 8 datoshi
    /// 59 : OpCode.EQUAL 	-> 32 datoshi
    /// 5A : OpCode.JMPIF_L 76000000 	-> 2 datoshi
    /// 5F : OpCode.LDLOC0 	-> 2 datoshi
    /// 60 : OpCode.PUSHDATA1 39 	-> 8 datoshi
    /// 63 : OpCode.EQUAL 	-> 32 datoshi
    /// 64 : OpCode.JMPIF 6E 	-> 2 datoshi
    /// 66 : OpCode.LDLOC0 	-> 2 datoshi
    /// 67 : OpCode.PUSHDATA1 3130 	-> 8 datoshi
    /// 6B : OpCode.EQUAL 	-> 32 datoshi
    /// 6C : OpCode.JMPIF 68 	-> 2 datoshi
    /// 6E : OpCode.LDLOC0 	-> 2 datoshi
    /// 6F : OpCode.PUSHDATA1 3131 	-> 8 datoshi
    /// 73 : OpCode.EQUAL 	-> 32 datoshi
    /// 74 : OpCode.JMPIF 62 	-> 2 datoshi
    /// 76 : OpCode.LDLOC0 	-> 2 datoshi
    /// 77 : OpCode.PUSHDATA1 3132 	-> 8 datoshi
    /// 7B : OpCode.EQUAL 	-> 32 datoshi
    /// 7C : OpCode.JMPIF 5C 	-> 2 datoshi
    /// 7E : OpCode.LDLOC0 	-> 2 datoshi
    /// 7F : OpCode.PUSHDATA1 3133 	-> 8 datoshi
    /// 83 : OpCode.EQUAL 	-> 32 datoshi
    /// 84 : OpCode.JMPIF 56 	-> 2 datoshi
    /// 86 : OpCode.LDLOC0 	-> 2 datoshi
    /// 87 : OpCode.PUSHDATA1 3134 	-> 8 datoshi
    /// 8B : OpCode.EQUAL 	-> 32 datoshi
    /// 8C : OpCode.JMPIF 50 	-> 2 datoshi
    /// 8E : OpCode.LDLOC0 	-> 2 datoshi
    /// 8F : OpCode.PUSHDATA1 3135 	-> 8 datoshi
    /// 93 : OpCode.EQUAL 	-> 32 datoshi
    /// 94 : OpCode.JMPIF 4A 	-> 2 datoshi
    /// 96 : OpCode.LDLOC0 	-> 2 datoshi
    /// 97 : OpCode.PUSHDATA1 3136 	-> 8 datoshi
    /// 9B : OpCode.EQUAL 	-> 32 datoshi
    /// 9C : OpCode.JMPIF 44 	-> 2 datoshi
    /// 9E : OpCode.LDLOC0 	-> 2 datoshi
    /// 9F : OpCode.PUSHDATA1 3137 	-> 8 datoshi
    /// A3 : OpCode.EQUAL 	-> 32 datoshi
    /// A4 : OpCode.JMPIF 3F 	-> 2 datoshi
    /// A6 : OpCode.LDLOC0 	-> 2 datoshi
    /// A7 : OpCode.PUSHDATA1 3138 	-> 8 datoshi
    /// AB : OpCode.EQUAL 	-> 32 datoshi
    /// AC : OpCode.JMPIF 3A 	-> 2 datoshi
    /// AE : OpCode.LDLOC0 	-> 2 datoshi
    /// AF : OpCode.PUSHDATA1 3139 	-> 8 datoshi
    /// B3 : OpCode.EQUAL 	-> 32 datoshi
    /// B4 : OpCode.JMPIF 35 	-> 2 datoshi
    /// B6 : OpCode.LDLOC0 	-> 2 datoshi
    /// B7 : OpCode.PUSHDATA1 3230 	-> 8 datoshi
    /// BB : OpCode.EQUAL 	-> 32 datoshi
    /// BC : OpCode.JMPIF 30 	-> 2 datoshi
    /// BE : OpCode.JMP 31 	-> 2 datoshi
    /// C0 : OpCode.PUSH1 	-> 1 datoshi
    /// C1 : OpCode.RET 	-> 0 datoshi
    /// C2 : OpCode.PUSH2 	-> 1 datoshi
    /// C3 : OpCode.RET 	-> 0 datoshi
    /// C4 : OpCode.PUSH3 	-> 1 datoshi
    /// C5 : OpCode.RET 	-> 0 datoshi
    /// C6 : OpCode.PUSH4 	-> 1 datoshi
    /// C7 : OpCode.RET 	-> 0 datoshi
    /// C8 : OpCode.PUSH5 	-> 1 datoshi
    /// C9 : OpCode.RET 	-> 0 datoshi
    /// CA : OpCode.PUSH6 	-> 1 datoshi
    /// CB : OpCode.RET 	-> 0 datoshi
    /// CC : OpCode.PUSH7 	-> 1 datoshi
    /// CD : OpCode.RET 	-> 0 datoshi
    /// CE : OpCode.PUSH8 	-> 1 datoshi
    /// CF : OpCode.RET 	-> 0 datoshi
    /// D0 : OpCode.PUSH9 	-> 1 datoshi
    /// D1 : OpCode.RET 	-> 0 datoshi
    /// D2 : OpCode.PUSH10 	-> 1 datoshi
    /// D3 : OpCode.RET 	-> 0 datoshi
    /// D4 : OpCode.PUSH11 	-> 1 datoshi
    /// D5 : OpCode.RET 	-> 0 datoshi
    /// D6 : OpCode.PUSH12 	-> 1 datoshi
    /// D7 : OpCode.RET 	-> 0 datoshi
    /// D8 : OpCode.PUSH13 	-> 1 datoshi
    /// D9 : OpCode.RET 	-> 0 datoshi
    /// DA : OpCode.PUSH14 	-> 1 datoshi
    /// DB : OpCode.RET 	-> 0 datoshi
    /// DC : OpCode.PUSH15 	-> 1 datoshi
    /// DD : OpCode.RET 	-> 0 datoshi
    /// DE : OpCode.PUSH16 	-> 1 datoshi
    /// DF : OpCode.RET 	-> 0 datoshi
    /// E0 : OpCode.PUSHINT8 11 	-> 1 datoshi
    /// E2 : OpCode.RET 	-> 0 datoshi
    /// E3 : OpCode.PUSHINT8 12 	-> 1 datoshi
    /// E5 : OpCode.RET 	-> 0 datoshi
    /// E6 : OpCode.PUSHINT8 13 	-> 1 datoshi
    /// E8 : OpCode.RET 	-> 0 datoshi
    /// E9 : OpCode.PUSHINT8 14 	-> 1 datoshi
    /// EB : OpCode.RET 	-> 0 datoshi
    /// EC : OpCode.PUSHINT8 15 	-> 1 datoshi
    /// EE : OpCode.RET 	-> 0 datoshi
    /// EF : OpCode.PUSHINT8 63 	-> 1 datoshi
    /// F1 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("switchLong")]
    public abstract object? SwitchLong(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEXB4cWkMYZckQGkMY5ckcWkMYpcloQAAAGkMZJclzwAAAGkMZZcl/AAAAGkMZpclJgEAAGkMZ5clUAEAACN/AQAAaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFI0sBAABoEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCMUAQAAaEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFI9wAAABoD6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCOlAAAAaGigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AibmgToEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wIjpoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCIGaBGhcGhA
    /// 0000 : OpCode.INITSLOT 0201 	-> 64 datoshi
    /// 0003 : OpCode.PUSH1 	-> 1 datoshi
    /// 0004 : OpCode.STLOC0 	-> 2 datoshi
    /// 0005 : OpCode.LDARG0 	-> 2 datoshi
    /// 0006 : OpCode.STLOC1 	-> 2 datoshi
    /// 0007 : OpCode.LDLOC1 	-> 2 datoshi
    /// 0008 : OpCode.PUSHDATA1 61 	-> 8 datoshi
    /// 000B : OpCode.EQUAL 	-> 32 datoshi
    /// 000C : OpCode.JMPIF 40 	-> 2 datoshi
    /// 000E : OpCode.LDLOC1 	-> 2 datoshi
    /// 000F : OpCode.PUSHDATA1 63 	-> 8 datoshi
    /// 0012 : OpCode.EQUAL 	-> 32 datoshi
    /// 0013 : OpCode.JMPIF 71 	-> 2 datoshi
    /// 0015 : OpCode.LDLOC1 	-> 2 datoshi
    /// 0016 : OpCode.PUSHDATA1 62 	-> 8 datoshi
    /// 0019 : OpCode.EQUAL 	-> 32 datoshi
    /// 001A : OpCode.JMPIF_L A1000000 	-> 2 datoshi
    /// 001F : OpCode.LDLOC1 	-> 2 datoshi
    /// 0020 : OpCode.PUSHDATA1 64 	-> 8 datoshi
    /// 0023 : OpCode.EQUAL 	-> 32 datoshi
    /// 0024 : OpCode.JMPIF_L CF000000 	-> 2 datoshi
    /// 0029 : OpCode.LDLOC1 	-> 2 datoshi
    /// 002A : OpCode.PUSHDATA1 65 	-> 8 datoshi
    /// 002D : OpCode.EQUAL 	-> 32 datoshi
    /// 002E : OpCode.JMPIF_L FC000000 	-> 2 datoshi
    /// 0033 : OpCode.LDLOC1 	-> 2 datoshi
    /// 0034 : OpCode.PUSHDATA1 66 	-> 8 datoshi
    /// 0037 : OpCode.EQUAL 	-> 32 datoshi
    /// 0038 : OpCode.JMPIF_L 26010000 	-> 2 datoshi
    /// 003D : OpCode.LDLOC1 	-> 2 datoshi
    /// 003E : OpCode.PUSHDATA1 67 	-> 8 datoshi
    /// 0041 : OpCode.EQUAL 	-> 32 datoshi
    /// 0042 : OpCode.JMPIF_L 50010000 	-> 2 datoshi
    /// 0047 : OpCode.JMP_L 7F010000 	-> 2 datoshi
    /// 004C : OpCode.LDLOC0 	-> 2 datoshi
    /// 004D : OpCode.DUP 	-> 2 datoshi
    /// 004E : OpCode.INC 	-> 4 datoshi
    /// 004F : OpCode.DUP 	-> 2 datoshi
    /// 0050 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 0055 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 0057 : OpCode.JMP 0A 	-> 2 datoshi
    /// 0059 : OpCode.DUP 	-> 2 datoshi
    /// 005A : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 005F : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 0061 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 006A : OpCode.AND 	-> 8 datoshi
    /// 006B : OpCode.DUP 	-> 2 datoshi
    /// 006C : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 0071 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 0073 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 007C : OpCode.SUB 	-> 8 datoshi
    /// 007D : OpCode.STLOC0 	-> 2 datoshi
    /// 007E : OpCode.DROP 	-> 2 datoshi
    /// 007F : OpCode.JMP_L 4B010000 	-> 2 datoshi
    /// 0084 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0085 : OpCode.PUSH2 	-> 1 datoshi
    /// 0086 : OpCode.MUL 	-> 8 datoshi
    /// 0087 : OpCode.DUP 	-> 2 datoshi
    /// 0088 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 008D : OpCode.JMPGE 04 	-> 2 datoshi
    /// 008F : OpCode.JMP 0A 	-> 2 datoshi
    /// 0091 : OpCode.DUP 	-> 2 datoshi
    /// 0092 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 0097 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 0099 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 00A2 : OpCode.AND 	-> 8 datoshi
    /// 00A3 : OpCode.DUP 	-> 2 datoshi
    /// 00A4 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 00A9 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 00AB : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 00B4 : OpCode.SUB 	-> 8 datoshi
    /// 00B5 : OpCode.STLOC0 	-> 2 datoshi
    /// 00B6 : OpCode.JMP_L 14010000 	-> 2 datoshi
    /// 00BB : OpCode.LDLOC0 	-> 2 datoshi
    /// 00BC : OpCode.DUP 	-> 2 datoshi
    /// 00BD : OpCode.DEC 	-> 4 datoshi
    /// 00BE : OpCode.DUP 	-> 2 datoshi
    /// 00BF : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 00C4 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 00C6 : OpCode.JMP 0A 	-> 2 datoshi
    /// 00C8 : OpCode.DUP 	-> 2 datoshi
    /// 00C9 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 00CE : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 00D0 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 00D9 : OpCode.AND 	-> 8 datoshi
    /// 00DA : OpCode.DUP 	-> 2 datoshi
    /// 00DB : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 00E0 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 00E2 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 00EB : OpCode.SUB 	-> 8 datoshi
    /// 00EC : OpCode.STLOC0 	-> 2 datoshi
    /// 00ED : OpCode.DROP 	-> 2 datoshi
    /// 00EE : OpCode.JMP_L DC000000 	-> 2 datoshi
    /// 00F3 : OpCode.LDLOC0 	-> 2 datoshi
    /// 00F4 : OpCode.PUSHM1 	-> 1 datoshi
    /// 00F5 : OpCode.MUL 	-> 8 datoshi
    /// 00F6 : OpCode.DUP 	-> 2 datoshi
    /// 00F7 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 00FC : OpCode.JMPGE 04 	-> 2 datoshi
    /// 00FE : OpCode.JMP 0A 	-> 2 datoshi
    /// 0100 : OpCode.DUP 	-> 2 datoshi
    /// 0101 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 0106 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 0108 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 0111 : OpCode.AND 	-> 8 datoshi
    /// 0112 : OpCode.DUP 	-> 2 datoshi
    /// 0113 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 0118 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 011A : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 0123 : OpCode.SUB 	-> 8 datoshi
    /// 0124 : OpCode.STLOC0 	-> 2 datoshi
    /// 0125 : OpCode.JMP_L A5000000 	-> 2 datoshi
    /// 012A : OpCode.LDLOC0 	-> 2 datoshi
    /// 012B : OpCode.LDLOC0 	-> 2 datoshi
    /// 012C : OpCode.MUL 	-> 8 datoshi
    /// 012D : OpCode.DUP 	-> 2 datoshi
    /// 012E : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 0133 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 0135 : OpCode.JMP 0A 	-> 2 datoshi
    /// 0137 : OpCode.DUP 	-> 2 datoshi
    /// 0138 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 013D : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 013F : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 0148 : OpCode.AND 	-> 8 datoshi
    /// 0149 : OpCode.DUP 	-> 2 datoshi
    /// 014A : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 014F : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 0151 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 015A : OpCode.SUB 	-> 8 datoshi
    /// 015B : OpCode.STLOC0 	-> 2 datoshi
    /// 015C : OpCode.JMP 6E 	-> 2 datoshi
    /// 015E : OpCode.LDLOC0 	-> 2 datoshi
    /// 015F : OpCode.PUSH3 	-> 1 datoshi
    /// 0160 : OpCode.MUL 	-> 8 datoshi
    /// 0161 : OpCode.DUP 	-> 2 datoshi
    /// 0162 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 0167 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 0169 : OpCode.JMP 0A 	-> 2 datoshi
    /// 016B : OpCode.DUP 	-> 2 datoshi
    /// 016C : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 0171 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 0173 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 017C : OpCode.AND 	-> 8 datoshi
    /// 017D : OpCode.DUP 	-> 2 datoshi
    /// 017E : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 0183 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 0185 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 018E : OpCode.SUB 	-> 8 datoshi
    /// 018F : OpCode.STLOC0 	-> 2 datoshi
    /// 0190 : OpCode.JMP 3A 	-> 2 datoshi
    /// 0192 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0193 : OpCode.PUSH2 	-> 1 datoshi
    /// 0194 : OpCode.ADD 	-> 8 datoshi
    /// 0195 : OpCode.DUP 	-> 2 datoshi
    /// 0196 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 019B : OpCode.JMPGE 04 	-> 2 datoshi
    /// 019D : OpCode.JMP 0A 	-> 2 datoshi
    /// 019F : OpCode.DUP 	-> 2 datoshi
    /// 01A0 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 01A5 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 01A7 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 01B0 : OpCode.AND 	-> 8 datoshi
    /// 01B1 : OpCode.DUP 	-> 2 datoshi
    /// 01B2 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 01B7 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 01B9 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 01C2 : OpCode.SUB 	-> 8 datoshi
    /// 01C3 : OpCode.STLOC0 	-> 2 datoshi
    /// 01C4 : OpCode.JMP 06 	-> 2 datoshi
    /// 01C6 : OpCode.LDLOC0 	-> 2 datoshi
    /// 01C7 : OpCode.PUSH1 	-> 1 datoshi
    /// 01C8 : OpCode.DIV 	-> 8 datoshi
    /// 01C9 : OpCode.STLOC0 	-> 2 datoshi
    /// 01CA : OpCode.LDLOC0 	-> 2 datoshi
    /// 01CB : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("switchLongLong")]
    public abstract object? SwitchLongLong(string? test);

    #endregion
}
