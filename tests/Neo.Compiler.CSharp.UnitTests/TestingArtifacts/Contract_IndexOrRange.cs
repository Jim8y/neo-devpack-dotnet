using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IndexOrRange(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IndexOrRange"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/a4BVxQADAoBAgMEBQYHCAkK2zBwaErKUBBRS5+McWgTUBBRS5+McmhKylASUUufjHNoFVATUUufjHRoSspQSsoSn1FLn4x1aErKE59QEFFLn4x2aErKFJ9QE1FLn4x3B2hKyhKfUErKFJ9RS5+MdwhoEM53CWnKNwAAQc/nR5ZqyjcAAEHP50eWa8o3AABBz+dHlmzKNwAAQc/nR5ZtyjcAAEHP50eWbso3AABBz+dHlm8HyjcAAEHP50eWbwjKNwAAQc/nR5ZvCTcAAEHP50eWDAkxMjM0NTY3ODl3Cm8KSspQEFFLn4zbKHcLbwoTUBBRS5+M2yh3DG8KSspQElFLn4zbKHcNbwoVUBNRS5+M2yh3Dm8KSspQSsoSn1FLn4zbKHcPbwpKyhOfUBBRS5+M2yh3EG8KSsoUn1ATUUufjNsodxFvCkrKEp9QSsoUn1FLn4zbKHcSbwoQzncTbwvbKEHP50eWbwzbKEHP50eWbw3bKEHP50eWbw7bKEHP50eWbw/bKEHP50eWbxDbKEHP50eWbxHbKEHP50eWbxLbKEHP50eWbxPbKEHP50eWQFm4evg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VxQADAECAwQFBgcICQrbMHBoSspQEFFLn4xxaBNQEFFLn4xyaErKUBJRS5+Mc2gVUBNRS5+MdGhKylBKyhKfUUufjHVoSsoTn1AQUUufjHZoSsoUn1ATUUufjHcHaErKEp9QSsoUn1FLn4x3CGgQzncJaco3AABBz+dHlmrKNwAAQc/nR5ZryjcAAEHP50eWbMo3AABBz+dHlm3KNwAAQc/nR5ZuyjcAAEHP50eWbwfKNwAAQc/nR5ZvCMo3AABBz+dHlm8JNwAAQc/nR5YMMTIzNDU2Nzg5dwpvCkrKUBBRS5+M2yh3C28KE1AQUUufjNsodwxvCkrKUBJRS5+M2yh3DW8KFVATUUufjNsodw5vCkrKUErKEp9RS5+M2yh3D28KSsoTn1AQUUufjNsodxBvCkrKFJ9QE1FLn4zbKHcRbwpKyhKfUErKFJ9RS5+M2yh3Em8KEM53E28L2yhBz+dHlm8M2yhBz+dHlm8N2yhBz+dHlm8O2yhBz+dHlm8P2yhBz+dHlm8Q2yhBz+dHlm8R2yhBz+dHlm8S2yhBz+dHlm8T2yhBz+dHlkA=
    /// 0000 : OpCode.INITSLOT 1400 	-> 64 datoshi
    /// 0003 : OpCode.PUSHDATA1 0102030405060708090A 	-> 8 datoshi
    /// 000F : OpCode.CONVERT 30 	-> 8192 datoshi
    /// 0011 : OpCode.STLOC0 	-> 2 datoshi
    /// 0012 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0013 : OpCode.DUP 	-> 2 datoshi
    /// 0014 : OpCode.SIZE 	-> 4 datoshi
    /// 0015 : OpCode.SWAP 	-> 2 datoshi
    /// 0016 : OpCode.PUSH0 	-> 1 datoshi
    /// 0017 : OpCode.ROT 	-> 2 datoshi
    /// 0018 : OpCode.OVER 	-> 2 datoshi
    /// 0019 : OpCode.SUB 	-> 8 datoshi
    /// 001A : OpCode.SUBSTR 	-> 2048 datoshi
    /// 001B : OpCode.STLOC1 	-> 2 datoshi
    /// 001C : OpCode.LDLOC0 	-> 2 datoshi
    /// 001D : OpCode.PUSH3 	-> 1 datoshi
    /// 001E : OpCode.SWAP 	-> 2 datoshi
    /// 001F : OpCode.PUSH0 	-> 1 datoshi
    /// 0020 : OpCode.ROT 	-> 2 datoshi
    /// 0021 : OpCode.OVER 	-> 2 datoshi
    /// 0022 : OpCode.SUB 	-> 8 datoshi
    /// 0023 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0024 : OpCode.STLOC2 	-> 2 datoshi
    /// 0025 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0026 : OpCode.DUP 	-> 2 datoshi
    /// 0027 : OpCode.SIZE 	-> 4 datoshi
    /// 0028 : OpCode.SWAP 	-> 2 datoshi
    /// 0029 : OpCode.PUSH2 	-> 1 datoshi
    /// 002A : OpCode.ROT 	-> 2 datoshi
    /// 002B : OpCode.OVER 	-> 2 datoshi
    /// 002C : OpCode.SUB 	-> 8 datoshi
    /// 002D : OpCode.SUBSTR 	-> 2048 datoshi
    /// 002E : OpCode.STLOC3 	-> 2 datoshi
    /// 002F : OpCode.LDLOC0 	-> 2 datoshi
    /// 0030 : OpCode.PUSH5 	-> 1 datoshi
    /// 0031 : OpCode.SWAP 	-> 2 datoshi
    /// 0032 : OpCode.PUSH3 	-> 1 datoshi
    /// 0033 : OpCode.ROT 	-> 2 datoshi
    /// 0034 : OpCode.OVER 	-> 2 datoshi
    /// 0035 : OpCode.SUB 	-> 8 datoshi
    /// 0036 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0037 : OpCode.STLOC4 	-> 2 datoshi
    /// 0038 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0039 : OpCode.DUP 	-> 2 datoshi
    /// 003A : OpCode.SIZE 	-> 4 datoshi
    /// 003B : OpCode.SWAP 	-> 2 datoshi
    /// 003C : OpCode.DUP 	-> 2 datoshi
    /// 003D : OpCode.SIZE 	-> 4 datoshi
    /// 003E : OpCode.PUSH2 	-> 1 datoshi
    /// 003F : OpCode.SUB 	-> 8 datoshi
    /// 0040 : OpCode.ROT 	-> 2 datoshi
    /// 0041 : OpCode.OVER 	-> 2 datoshi
    /// 0042 : OpCode.SUB 	-> 8 datoshi
    /// 0043 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0044 : OpCode.STLOC5 	-> 2 datoshi
    /// 0045 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0046 : OpCode.DUP 	-> 2 datoshi
    /// 0047 : OpCode.SIZE 	-> 4 datoshi
    /// 0048 : OpCode.PUSH3 	-> 1 datoshi
    /// 0049 : OpCode.SUB 	-> 8 datoshi
    /// 004A : OpCode.SWAP 	-> 2 datoshi
    /// 004B : OpCode.PUSH0 	-> 1 datoshi
    /// 004C : OpCode.ROT 	-> 2 datoshi
    /// 004D : OpCode.OVER 	-> 2 datoshi
    /// 004E : OpCode.SUB 	-> 8 datoshi
    /// 004F : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0050 : OpCode.STLOC6 	-> 2 datoshi
    /// 0051 : OpCode.LDLOC0 	-> 2 datoshi
    /// 0052 : OpCode.DUP 	-> 2 datoshi
    /// 0053 : OpCode.SIZE 	-> 4 datoshi
    /// 0054 : OpCode.PUSH4 	-> 1 datoshi
    /// 0055 : OpCode.SUB 	-> 8 datoshi
    /// 0056 : OpCode.SWAP 	-> 2 datoshi
    /// 0057 : OpCode.PUSH3 	-> 1 datoshi
    /// 0058 : OpCode.ROT 	-> 2 datoshi
    /// 0059 : OpCode.OVER 	-> 2 datoshi
    /// 005A : OpCode.SUB 	-> 8 datoshi
    /// 005B : OpCode.SUBSTR 	-> 2048 datoshi
    /// 005C : OpCode.STLOC 07 	-> 2 datoshi
    /// 005E : OpCode.LDLOC0 	-> 2 datoshi
    /// 005F : OpCode.DUP 	-> 2 datoshi
    /// 0060 : OpCode.SIZE 	-> 4 datoshi
    /// 0061 : OpCode.PUSH2 	-> 1 datoshi
    /// 0062 : OpCode.SUB 	-> 8 datoshi
    /// 0063 : OpCode.SWAP 	-> 2 datoshi
    /// 0064 : OpCode.DUP 	-> 2 datoshi
    /// 0065 : OpCode.SIZE 	-> 4 datoshi
    /// 0066 : OpCode.PUSH4 	-> 1 datoshi
    /// 0067 : OpCode.SUB 	-> 8 datoshi
    /// 0068 : OpCode.ROT 	-> 2 datoshi
    /// 0069 : OpCode.OVER 	-> 2 datoshi
    /// 006A : OpCode.SUB 	-> 8 datoshi
    /// 006B : OpCode.SUBSTR 	-> 2048 datoshi
    /// 006C : OpCode.STLOC 08 	-> 2 datoshi
    /// 006E : OpCode.LDLOC0 	-> 2 datoshi
    /// 006F : OpCode.PUSH0 	-> 1 datoshi
    /// 0070 : OpCode.PICKITEM 	-> 64 datoshi
    /// 0071 : OpCode.STLOC 09 	-> 2 datoshi
    /// 0073 : OpCode.LDLOC1 	-> 2 datoshi
    /// 0074 : OpCode.SIZE 	-> 4 datoshi
    /// 0075 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 0078 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 007D : OpCode.LDLOC2 	-> 2 datoshi
    /// 007E : OpCode.SIZE 	-> 4 datoshi
    /// 007F : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 0082 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0087 : OpCode.LDLOC3 	-> 2 datoshi
    /// 0088 : OpCode.SIZE 	-> 4 datoshi
    /// 0089 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 008C : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0091 : OpCode.LDLOC4 	-> 2 datoshi
    /// 0092 : OpCode.SIZE 	-> 4 datoshi
    /// 0093 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 0096 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 009B : OpCode.LDLOC5 	-> 2 datoshi
    /// 009C : OpCode.SIZE 	-> 4 datoshi
    /// 009D : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 00A0 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 00A5 : OpCode.LDLOC6 	-> 2 datoshi
    /// 00A6 : OpCode.SIZE 	-> 4 datoshi
    /// 00A7 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 00AA : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 00AF : OpCode.LDLOC 07 	-> 2 datoshi
    /// 00B1 : OpCode.SIZE 	-> 4 datoshi
    /// 00B2 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 00B5 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 00BA : OpCode.LDLOC 08 	-> 2 datoshi
    /// 00BC : OpCode.SIZE 	-> 4 datoshi
    /// 00BD : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 00C0 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 00C5 : OpCode.LDLOC 09 	-> 2 datoshi
    /// 00C7 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 00CA : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 00CF : OpCode.PUSHDATA1 313233343536373839 	-> 8 datoshi
    /// 00DA : OpCode.STLOC 0A 	-> 2 datoshi
    /// 00DC : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 00DE : OpCode.DUP 	-> 2 datoshi
    /// 00DF : OpCode.SIZE 	-> 4 datoshi
    /// 00E0 : OpCode.SWAP 	-> 2 datoshi
    /// 00E1 : OpCode.PUSH0 	-> 1 datoshi
    /// 00E2 : OpCode.ROT 	-> 2 datoshi
    /// 00E3 : OpCode.OVER 	-> 2 datoshi
    /// 00E4 : OpCode.SUB 	-> 8 datoshi
    /// 00E5 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 00E6 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 00E8 : OpCode.STLOC 0B 	-> 2 datoshi
    /// 00EA : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 00EC : OpCode.PUSH3 	-> 1 datoshi
    /// 00ED : OpCode.SWAP 	-> 2 datoshi
    /// 00EE : OpCode.PUSH0 	-> 1 datoshi
    /// 00EF : OpCode.ROT 	-> 2 datoshi
    /// 00F0 : OpCode.OVER 	-> 2 datoshi
    /// 00F1 : OpCode.SUB 	-> 8 datoshi
    /// 00F2 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 00F3 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 00F5 : OpCode.STLOC 0C 	-> 2 datoshi
    /// 00F7 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 00F9 : OpCode.DUP 	-> 2 datoshi
    /// 00FA : OpCode.SIZE 	-> 4 datoshi
    /// 00FB : OpCode.SWAP 	-> 2 datoshi
    /// 00FC : OpCode.PUSH2 	-> 1 datoshi
    /// 00FD : OpCode.ROT 	-> 2 datoshi
    /// 00FE : OpCode.OVER 	-> 2 datoshi
    /// 00FF : OpCode.SUB 	-> 8 datoshi
    /// 0100 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0101 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0103 : OpCode.STLOC 0D 	-> 2 datoshi
    /// 0105 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 0107 : OpCode.PUSH5 	-> 1 datoshi
    /// 0108 : OpCode.SWAP 	-> 2 datoshi
    /// 0109 : OpCode.PUSH3 	-> 1 datoshi
    /// 010A : OpCode.ROT 	-> 2 datoshi
    /// 010B : OpCode.OVER 	-> 2 datoshi
    /// 010C : OpCode.SUB 	-> 8 datoshi
    /// 010D : OpCode.SUBSTR 	-> 2048 datoshi
    /// 010E : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0110 : OpCode.STLOC 0E 	-> 2 datoshi
    /// 0112 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 0114 : OpCode.DUP 	-> 2 datoshi
    /// 0115 : OpCode.SIZE 	-> 4 datoshi
    /// 0116 : OpCode.SWAP 	-> 2 datoshi
    /// 0117 : OpCode.DUP 	-> 2 datoshi
    /// 0118 : OpCode.SIZE 	-> 4 datoshi
    /// 0119 : OpCode.PUSH2 	-> 1 datoshi
    /// 011A : OpCode.SUB 	-> 8 datoshi
    /// 011B : OpCode.ROT 	-> 2 datoshi
    /// 011C : OpCode.OVER 	-> 2 datoshi
    /// 011D : OpCode.SUB 	-> 8 datoshi
    /// 011E : OpCode.SUBSTR 	-> 2048 datoshi
    /// 011F : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0121 : OpCode.STLOC 0F 	-> 2 datoshi
    /// 0123 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 0125 : OpCode.DUP 	-> 2 datoshi
    /// 0126 : OpCode.SIZE 	-> 4 datoshi
    /// 0127 : OpCode.PUSH3 	-> 1 datoshi
    /// 0128 : OpCode.SUB 	-> 8 datoshi
    /// 0129 : OpCode.SWAP 	-> 2 datoshi
    /// 012A : OpCode.PUSH0 	-> 1 datoshi
    /// 012B : OpCode.ROT 	-> 2 datoshi
    /// 012C : OpCode.OVER 	-> 2 datoshi
    /// 012D : OpCode.SUB 	-> 8 datoshi
    /// 012E : OpCode.SUBSTR 	-> 2048 datoshi
    /// 012F : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0131 : OpCode.STLOC 10 	-> 2 datoshi
    /// 0133 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 0135 : OpCode.DUP 	-> 2 datoshi
    /// 0136 : OpCode.SIZE 	-> 4 datoshi
    /// 0137 : OpCode.PUSH4 	-> 1 datoshi
    /// 0138 : OpCode.SUB 	-> 8 datoshi
    /// 0139 : OpCode.SWAP 	-> 2 datoshi
    /// 013A : OpCode.PUSH3 	-> 1 datoshi
    /// 013B : OpCode.ROT 	-> 2 datoshi
    /// 013C : OpCode.OVER 	-> 2 datoshi
    /// 013D : OpCode.SUB 	-> 8 datoshi
    /// 013E : OpCode.SUBSTR 	-> 2048 datoshi
    /// 013F : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0141 : OpCode.STLOC 11 	-> 2 datoshi
    /// 0143 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 0145 : OpCode.DUP 	-> 2 datoshi
    /// 0146 : OpCode.SIZE 	-> 4 datoshi
    /// 0147 : OpCode.PUSH2 	-> 1 datoshi
    /// 0148 : OpCode.SUB 	-> 8 datoshi
    /// 0149 : OpCode.SWAP 	-> 2 datoshi
    /// 014A : OpCode.DUP 	-> 2 datoshi
    /// 014B : OpCode.SIZE 	-> 4 datoshi
    /// 014C : OpCode.PUSH4 	-> 1 datoshi
    /// 014D : OpCode.SUB 	-> 8 datoshi
    /// 014E : OpCode.ROT 	-> 2 datoshi
    /// 014F : OpCode.OVER 	-> 2 datoshi
    /// 0150 : OpCode.SUB 	-> 8 datoshi
    /// 0151 : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0152 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0154 : OpCode.STLOC 12 	-> 2 datoshi
    /// 0156 : OpCode.LDLOC 0A 	-> 2 datoshi
    /// 0158 : OpCode.PUSH0 	-> 1 datoshi
    /// 0159 : OpCode.PICKITEM 	-> 64 datoshi
    /// 015A : OpCode.STLOC 13 	-> 2 datoshi
    /// 015C : OpCode.LDLOC 0B 	-> 2 datoshi
    /// 015E : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0160 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0165 : OpCode.LDLOC 0C 	-> 2 datoshi
    /// 0167 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0169 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 016E : OpCode.LDLOC 0D 	-> 2 datoshi
    /// 0170 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0172 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0177 : OpCode.LDLOC 0E 	-> 2 datoshi
    /// 0179 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 017B : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0180 : OpCode.LDLOC 0F 	-> 2 datoshi
    /// 0182 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0184 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0189 : OpCode.LDLOC 10 	-> 2 datoshi
    /// 018B : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 018D : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0192 : OpCode.LDLOC 11 	-> 2 datoshi
    /// 0194 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0196 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 019B : OpCode.LDLOC 12 	-> 2 datoshi
    /// 019D : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 019F : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 01A4 : OpCode.LDLOC 13 	-> 2 datoshi
    /// 01A6 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 01A8 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 01AD : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion
}
