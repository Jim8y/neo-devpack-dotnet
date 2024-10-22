using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":84,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":136,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":145,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":201,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":303,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":341,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":418,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":519,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":580,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":635,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":836,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":938,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":1065,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":1143,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":1189,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":1260,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1333,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAwAAZKeztuANAgDKmjsCQEIPAAEQJxTAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHINT64 000064A7B3B6E00D 	-> 1 datoshi
    /// 0C : OpCode.PUSHINT32 00CA9A3B 	-> 1 datoshi
    /// 11 : OpCode.PUSHINT32 40420F00 	-> 1 datoshi
    /// 16 : OpCode.PUSHINT16 1027 	-> 1 datoshi
    /// 19 : OpCode.PUSH4 	-> 1 datoshi
    /// 1A : OpCode.PACK 	-> 2048 datoshi
    /// 1B : OpCode.STLOC0 	-> 2 datoshi
    /// 1C : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 1D : OpCode.STLOC1 	-> 2 datoshi
    /// 1E : OpCode.LDLOC0 	-> 2 datoshi
    /// 1F : OpCode.DUP 	-> 2 datoshi
    /// 20 : OpCode.STLOC2 	-> 2 datoshi
    /// 21 : OpCode.SIZE 	-> 4 datoshi
    /// 22 : OpCode.STLOC3 	-> 2 datoshi
    /// 23 : OpCode.PUSH0 	-> 1 datoshi
    /// 24 : OpCode.STLOC4 	-> 2 datoshi
    /// 25 : OpCode.JMP 0C 	-> 2 datoshi
    /// 27 : OpCode.LDLOC2 	-> 2 datoshi
    /// 28 : OpCode.LDLOC4 	-> 2 datoshi
    /// 29 : OpCode.PICKITEM 	-> 64 datoshi
    /// 2A : OpCode.STLOC5 	-> 2 datoshi
    /// 2B : OpCode.LDLOC1 	-> 2 datoshi
    /// 2C : OpCode.LDLOC5 	-> 2 datoshi
    /// 2D : OpCode.APPEND 	-> 8192 datoshi
    /// 2E : OpCode.LDLOC4 	-> 2 datoshi
    /// 2F : OpCode.INC 	-> 4 datoshi
    /// 30 : OpCode.STLOC4 	-> 2 datoshi
    /// 31 : OpCode.LDLOC4 	-> 2 datoshi
    /// 32 : OpCode.LDLOC3 	-> 2 datoshi
    /// 33 : OpCode.JMPLT F4 	-> 2 datoshi
    /// 35 : OpCode.LDLOC1 	-> 2 datoshi
    /// 36 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAEKEdswcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 010A11 	-> 8 datoshi
    /// 08 : OpCode.CONVERT 30 	-> 8192 datoshi
    /// 0A : OpCode.STLOC0 	-> 2 datoshi
    /// 0B : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 0C : OpCode.STLOC1 	-> 2 datoshi
    /// 0D : OpCode.LDLOC0 	-> 2 datoshi
    /// 0E : OpCode.DUP 	-> 2 datoshi
    /// 0F : OpCode.STLOC2 	-> 2 datoshi
    /// 10 : OpCode.SIZE 	-> 4 datoshi
    /// 11 : OpCode.STLOC3 	-> 2 datoshi
    /// 12 : OpCode.PUSH0 	-> 1 datoshi
    /// 13 : OpCode.STLOC4 	-> 2 datoshi
    /// 14 : OpCode.JMP 0C 	-> 2 datoshi
    /// 16 : OpCode.LDLOC2 	-> 2 datoshi
    /// 17 : OpCode.LDLOC4 	-> 2 datoshi
    /// 18 : OpCode.PICKITEM 	-> 64 datoshi
    /// 19 : OpCode.STLOC5 	-> 2 datoshi
    /// 1A : OpCode.LDLOC1 	-> 2 datoshi
    /// 1B : OpCode.LDLOC5 	-> 2 datoshi
    /// 1C : OpCode.APPEND 	-> 8192 datoshi
    /// 1D : OpCode.LDLOC4 	-> 2 datoshi
    /// 1E : OpCode.INC 	-> 4 datoshi
    /// 1F : OpCode.STLOC4 	-> 2 datoshi
    /// 20 : OpCode.LDLOC4 	-> 2 datoshi
    /// 21 : OpCode.LDLOC3 	-> 2 datoshi
    /// 22 : OpCode.JMPLT F4 	-> 2 datoshi
    /// 24 : OpCode.LDLOC1 	-> 2 datoshi
    /// 25 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADHBoykA=
    /// 00 : OpCode.INITSLOT 0100 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 	-> 8 datoshi
    /// 05 : OpCode.STLOC0 	-> 2 datoshi
    /// 06 : OpCode.LDLOC0 	-> 2 datoshi
    /// 07 : OpCode.SIZE 	-> 4 datoshi
    /// 08 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAwMaGlqDGRlZgxhYmMVwHAMcWhKcspzEHQiD2psznVpbYvbKHFsnHRsazDxaUA=
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 	-> 8 datoshi
    /// 05 : OpCode.PUSHDATA1 	-> 8 datoshi
    /// 07 : OpCode.PUSHDATA1 68696A 	-> 8 datoshi
    /// 0C : OpCode.PUSHDATA1 646566 	-> 8 datoshi
    /// 11 : OpCode.PUSHDATA1 616263 	-> 8 datoshi
    /// 16 : OpCode.PUSH5 	-> 1 datoshi
    /// 17 : OpCode.PACK 	-> 2048 datoshi
    /// 18 : OpCode.STLOC0 	-> 2 datoshi
    /// 19 : OpCode.PUSHDATA1 	-> 8 datoshi
    /// 1B : OpCode.STLOC1 	-> 2 datoshi
    /// 1C : OpCode.LDLOC0 	-> 2 datoshi
    /// 1D : OpCode.DUP 	-> 2 datoshi
    /// 1E : OpCode.STLOC2 	-> 2 datoshi
    /// 1F : OpCode.SIZE 	-> 4 datoshi
    /// 20 : OpCode.STLOC3 	-> 2 datoshi
    /// 21 : OpCode.PUSH0 	-> 1 datoshi
    /// 22 : OpCode.STLOC4 	-> 2 datoshi
    /// 23 : OpCode.JMP 0F 	-> 2 datoshi
    /// 25 : OpCode.LDLOC2 	-> 2 datoshi
    /// 26 : OpCode.LDLOC4 	-> 2 datoshi
    /// 27 : OpCode.PICKITEM 	-> 64 datoshi
    /// 28 : OpCode.STLOC5 	-> 2 datoshi
    /// 29 : OpCode.LDLOC1 	-> 2 datoshi
    /// 2A : OpCode.LDLOC5 	-> 2 datoshi
    /// 2B : OpCode.CAT 	-> 2048 datoshi
    /// 2C : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 2E : OpCode.STLOC1 	-> 2 datoshi
    /// 2F : OpCode.LDLOC4 	-> 2 datoshi
    /// 30 : OpCode.INC 	-> 4 datoshi
    /// 31 : OpCode.STLOC4 	-> 2 datoshi
    /// 32 : OpCode.LDLOC4 	-> 2 datoshi
    /// 33 : OpCode.LDLOC3 	-> 2 datoshi
    /// 34 : OpCode.JMPLT F1 	-> 2 datoshi
    /// 36 : OpCode.LDLOC1 	-> 2 datoshi
    /// 37 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAWNsoStgkCUrKACEoAzpY2yhK2CQJSsoAISgDOhLAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.LDSFLD0 	-> 2 datoshi
    /// 04 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 06 : OpCode.DUP 	-> 2 datoshi
    /// 07 : OpCode.ISNULL 	-> 2 datoshi
    /// 08 : OpCode.JMPIF 09 	-> 2 datoshi
    /// 0A : OpCode.DUP 	-> 2 datoshi
    /// 0B : OpCode.SIZE 	-> 4 datoshi
    /// 0C : OpCode.PUSHINT8 21 	-> 1 datoshi
    /// 0E : OpCode.JMPEQ 03 	-> 2 datoshi
    /// 10 : OpCode.THROW 	-> 512 datoshi
    /// 11 : OpCode.LDSFLD0 	-> 2 datoshi
    /// 12 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 14 : OpCode.DUP 	-> 2 datoshi
    /// 15 : OpCode.ISNULL 	-> 2 datoshi
    /// 16 : OpCode.JMPIF 09 	-> 2 datoshi
    /// 18 : OpCode.DUP 	-> 2 datoshi
    /// 19 : OpCode.SIZE 	-> 4 datoshi
    /// 1A : OpCode.PUSHINT8 21 	-> 1 datoshi
    /// 1C : OpCode.JMPEQ 03 	-> 2 datoshi
    /// 1E : OpCode.THROW 	-> 512 datoshi
    /// 1F : OpCode.PUSH2 	-> 1 datoshi
    /// 20 : OpCode.PACK 	-> 2048 datoshi
    /// 21 : OpCode.STLOC0 	-> 2 datoshi
    /// 22 : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 23 : OpCode.STLOC1 	-> 2 datoshi
    /// 24 : OpCode.LDLOC0 	-> 2 datoshi
    /// 25 : OpCode.DUP 	-> 2 datoshi
    /// 26 : OpCode.STLOC2 	-> 2 datoshi
    /// 27 : OpCode.SIZE 	-> 4 datoshi
    /// 28 : OpCode.STLOC3 	-> 2 datoshi
    /// 29 : OpCode.PUSH0 	-> 1 datoshi
    /// 2A : OpCode.STLOC4 	-> 2 datoshi
    /// 2B : OpCode.JMP 0C 	-> 2 datoshi
    /// 2D : OpCode.LDLOC2 	-> 2 datoshi
    /// 2E : OpCode.LDLOC4 	-> 2 datoshi
    /// 2F : OpCode.PICKITEM 	-> 64 datoshi
    /// 30 : OpCode.STLOC5 	-> 2 datoshi
    /// 31 : OpCode.LDLOC1 	-> 2 datoshi
    /// 32 : OpCode.LDLOC5 	-> 2 datoshi
    /// 33 : OpCode.APPEND 	-> 8192 datoshi
    /// 34 : OpCode.LDLOC4 	-> 2 datoshi
    /// 35 : OpCode.INC 	-> 4 datoshi
    /// 36 : OpCode.STLOC4 	-> 2 datoshi
    /// 37 : OpCode.LDLOC4 	-> 2 datoshi
    /// 38 : OpCode.LDLOC3 	-> 2 datoshi
    /// 39 : OpCode.JMPLT F4 	-> 2 datoshi
    /// 3B : OpCode.LDLOC1 	-> 2 datoshi
    /// 3C : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFBMSERTAcBBxaEpyynMQdCI7amzOdWltnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswxWlA
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSH4 	-> 1 datoshi
    /// 04 : OpCode.PUSH3 	-> 1 datoshi
    /// 05 : OpCode.PUSH2 	-> 1 datoshi
    /// 06 : OpCode.PUSH1 	-> 1 datoshi
    /// 07 : OpCode.PUSH4 	-> 1 datoshi
    /// 08 : OpCode.PACK 	-> 2048 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.PUSH0 	-> 1 datoshi
    /// 0B : OpCode.STLOC1 	-> 2 datoshi
    /// 0C : OpCode.LDLOC0 	-> 2 datoshi
    /// 0D : OpCode.DUP 	-> 2 datoshi
    /// 0E : OpCode.STLOC2 	-> 2 datoshi
    /// 0F : OpCode.SIZE 	-> 4 datoshi
    /// 10 : OpCode.STLOC3 	-> 2 datoshi
    /// 11 : OpCode.PUSH0 	-> 1 datoshi
    /// 12 : OpCode.STLOC4 	-> 2 datoshi
    /// 13 : OpCode.JMP 3B 	-> 2 datoshi
    /// 15 : OpCode.LDLOC2 	-> 2 datoshi
    /// 16 : OpCode.LDLOC4 	-> 2 datoshi
    /// 17 : OpCode.PICKITEM 	-> 64 datoshi
    /// 18 : OpCode.STLOC5 	-> 2 datoshi
    /// 19 : OpCode.LDLOC1 	-> 2 datoshi
    /// 1A : OpCode.LDLOC5 	-> 2 datoshi
    /// 1B : OpCode.ADD 	-> 8 datoshi
    /// 1C : OpCode.DUP 	-> 2 datoshi
    /// 1D : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 22 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 24 : OpCode.JMP 0A 	-> 2 datoshi
    /// 26 : OpCode.DUP 	-> 2 datoshi
    /// 27 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 2C : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 37 : OpCode.AND 	-> 8 datoshi
    /// 38 : OpCode.DUP 	-> 2 datoshi
    /// 39 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 3E : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 40 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 49 : OpCode.SUB 	-> 8 datoshi
    /// 4A : OpCode.STLOC1 	-> 2 datoshi
    /// 4B : OpCode.LDLOC4 	-> 2 datoshi
    /// 4C : OpCode.INC 	-> 4 datoshi
    /// 4D : OpCode.STLOC4 	-> 2 datoshi
    /// 4E : OpCode.LDLOC4 	-> 2 datoshi
    /// 4F : OpCode.LDLOC3 	-> 2 datoshi
    /// 50 : OpCode.JMPLT C5 	-> 2 datoshi
    /// 52 : OpCode.LDLOC1 	-> 2 datoshi
    /// 53 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBFBMSERTAcBBxPIkAAAAAAAAAaEpyynMQdCJzamzOdXhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AELYmBCI7aW2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FsnHRsazCNPQVyPQJpQA==
    /// 00 : OpCode.INITSLOT 0601 	-> 64 datoshi
    /// 03 : OpCode.PUSH4 	-> 1 datoshi
    /// 04 : OpCode.PUSH3 	-> 1 datoshi
    /// 05 : OpCode.PUSH2 	-> 1 datoshi
    /// 06 : OpCode.PUSH1 	-> 1 datoshi
    /// 07 : OpCode.PUSH4 	-> 1 datoshi
    /// 08 : OpCode.PACK 	-> 2048 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.PUSH0 	-> 1 datoshi
    /// 0B : OpCode.STLOC1 	-> 2 datoshi
    /// 0C : OpCode.TRY_L 8900000000000000 	-> 4 datoshi
    /// 15 : OpCode.LDLOC0 	-> 2 datoshi
    /// 16 : OpCode.DUP 	-> 2 datoshi
    /// 17 : OpCode.STLOC2 	-> 2 datoshi
    /// 18 : OpCode.SIZE 	-> 4 datoshi
    /// 19 : OpCode.STLOC3 	-> 2 datoshi
    /// 1A : OpCode.PUSH0 	-> 1 datoshi
    /// 1B : OpCode.STLOC4 	-> 2 datoshi
    /// 1C : OpCode.JMP 73 	-> 2 datoshi
    /// 1E : OpCode.LDLOC2 	-> 2 datoshi
    /// 1F : OpCode.LDLOC4 	-> 2 datoshi
    /// 20 : OpCode.PICKITEM 	-> 64 datoshi
    /// 21 : OpCode.STLOC5 	-> 2 datoshi
    /// 22 : OpCode.LDARG0 	-> 2 datoshi
    /// 23 : OpCode.DUP 	-> 2 datoshi
    /// 24 : OpCode.DEC 	-> 4 datoshi
    /// 25 : OpCode.DUP 	-> 2 datoshi
    /// 26 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 2B : OpCode.JMPGE 04 	-> 2 datoshi
    /// 2D : OpCode.JMP 0A 	-> 2 datoshi
    /// 2F : OpCode.DUP 	-> 2 datoshi
    /// 30 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 35 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 37 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 40 : OpCode.AND 	-> 8 datoshi
    /// 41 : OpCode.DUP 	-> 2 datoshi
    /// 42 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 47 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 49 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 52 : OpCode.SUB 	-> 8 datoshi
    /// 53 : OpCode.STARG0 	-> 2 datoshi
    /// 54 : OpCode.PUSH0 	-> 1 datoshi
    /// 55 : OpCode.LE 	-> 8 datoshi
    /// 56 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 58 : OpCode.JMP 3B 	-> 2 datoshi
    /// 5A : OpCode.LDLOC1 	-> 2 datoshi
    /// 5B : OpCode.LDLOC5 	-> 2 datoshi
    /// 5C : OpCode.ADD 	-> 8 datoshi
    /// 5D : OpCode.DUP 	-> 2 datoshi
    /// 5E : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 63 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 65 : OpCode.JMP 0A 	-> 2 datoshi
    /// 67 : OpCode.DUP 	-> 2 datoshi
    /// 68 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 6D : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 6F : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 78 : OpCode.AND 	-> 8 datoshi
    /// 79 : OpCode.DUP 	-> 2 datoshi
    /// 7A : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 7F : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 81 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 8A : OpCode.SUB 	-> 8 datoshi
    /// 8B : OpCode.STLOC1 	-> 2 datoshi
    /// 8C : OpCode.LDLOC4 	-> 2 datoshi
    /// 8D : OpCode.INC 	-> 4 datoshi
    /// 8E : OpCode.STLOC4 	-> 2 datoshi
    /// 8F : OpCode.LDLOC4 	-> 2 datoshi
    /// 90 : OpCode.LDLOC3 	-> 2 datoshi
    /// 91 : OpCode.JMPLT 8D 	-> 2 datoshi
    /// 93 : OpCode.ENDTRY 05 	-> 4 datoshi
    /// 95 : OpCode.STLOC2 	-> 2 datoshi
    /// 96 : OpCode.ENDTRY 02 	-> 4 datoshi
    /// 98 : OpCode.LDLOC1 	-> 2 datoshi
    /// 99 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAFBMSERTAcBBxEHIiaWloas6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqaMq1JJVpQA==
    /// 00 : OpCode.INITSLOT 0300 	-> 64 datoshi
    /// 03 : OpCode.PUSH4 	-> 1 datoshi
    /// 04 : OpCode.PUSH3 	-> 1 datoshi
    /// 05 : OpCode.PUSH2 	-> 1 datoshi
    /// 06 : OpCode.PUSH1 	-> 1 datoshi
    /// 07 : OpCode.PUSH4 	-> 1 datoshi
    /// 08 : OpCode.PACK 	-> 2048 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.PUSH0 	-> 1 datoshi
    /// 0B : OpCode.STLOC1 	-> 2 datoshi
    /// 0C : OpCode.PUSH0 	-> 1 datoshi
    /// 0D : OpCode.STLOC2 	-> 2 datoshi
    /// 0E : OpCode.JMP 69 	-> 2 datoshi
    /// 10 : OpCode.LDLOC1 	-> 2 datoshi
    /// 11 : OpCode.LDLOC0 	-> 2 datoshi
    /// 12 : OpCode.LDLOC2 	-> 2 datoshi
    /// 13 : OpCode.PICKITEM 	-> 64 datoshi
    /// 14 : OpCode.ADD 	-> 8 datoshi
    /// 15 : OpCode.DUP 	-> 2 datoshi
    /// 16 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 1B : OpCode.JMPGE 04 	-> 2 datoshi
    /// 1D : OpCode.JMP 0A 	-> 2 datoshi
    /// 1F : OpCode.DUP 	-> 2 datoshi
    /// 20 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 25 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 27 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 30 : OpCode.AND 	-> 8 datoshi
    /// 31 : OpCode.DUP 	-> 2 datoshi
    /// 32 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 37 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 39 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 42 : OpCode.SUB 	-> 8 datoshi
    /// 43 : OpCode.STLOC1 	-> 2 datoshi
    /// 44 : OpCode.LDLOC2 	-> 2 datoshi
    /// 45 : OpCode.DUP 	-> 2 datoshi
    /// 46 : OpCode.INC 	-> 4 datoshi
    /// 47 : OpCode.DUP 	-> 2 datoshi
    /// 48 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 4D : OpCode.JMPGE 04 	-> 2 datoshi
    /// 4F : OpCode.JMP 0A 	-> 2 datoshi
    /// 51 : OpCode.DUP 	-> 2 datoshi
    /// 52 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 57 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 59 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 62 : OpCode.AND 	-> 8 datoshi
    /// 63 : OpCode.DUP 	-> 2 datoshi
    /// 64 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 69 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 6B : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 74 : OpCode.SUB 	-> 8 datoshi
    /// 75 : OpCode.STLOC2 	-> 2 datoshi
    /// 76 : OpCode.DROP 	-> 2 datoshi
    /// 77 : OpCode.LDLOC2 	-> 2 datoshi
    /// 78 : OpCode.LDLOC0 	-> 2 datoshi
    /// 79 : OpCode.SIZE 	-> 4 datoshi
    /// 7A : OpCode.LT 	-> 8 datoshi
    /// 7B : OpCode.JMPIF 95 	-> 2 datoshi
    /// 7D : OpCode.LDLOC1 	-> 2 datoshi
    /// 7E : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAHsMdGVzdAwBAtswE8BwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 05 : OpCode.PUSHDATA1 74657374 	-> 8 datoshi
    /// 0B : OpCode.PUSHDATA1 0102 	-> 8 datoshi
    /// 0F : OpCode.CONVERT 30 	-> 8192 datoshi
    /// 11 : OpCode.PUSH3 	-> 1 datoshi
    /// 12 : OpCode.PACK 	-> 2048 datoshi
    /// 13 : OpCode.STLOC0 	-> 2 datoshi
    /// 14 : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 15 : OpCode.STLOC1 	-> 2 datoshi
    /// 16 : OpCode.LDLOC0 	-> 2 datoshi
    /// 17 : OpCode.DUP 	-> 2 datoshi
    /// 18 : OpCode.STLOC2 	-> 2 datoshi
    /// 19 : OpCode.SIZE 	-> 4 datoshi
    /// 1A : OpCode.STLOC3 	-> 2 datoshi
    /// 1B : OpCode.PUSH0 	-> 1 datoshi
    /// 1C : OpCode.STLOC4 	-> 2 datoshi
    /// 1D : OpCode.JMP 0C 	-> 2 datoshi
    /// 1F : OpCode.LDLOC2 	-> 2 datoshi
    /// 20 : OpCode.LDLOC4 	-> 2 datoshi
    /// 21 : OpCode.PICKITEM 	-> 64 datoshi
    /// 22 : OpCode.STLOC5 	-> 2 datoshi
    /// 23 : OpCode.LDLOC1 	-> 2 datoshi
    /// 24 : OpCode.LDLOC5 	-> 2 datoshi
    /// 25 : OpCode.APPEND 	-> 8192 datoshi
    /// 26 : OpCode.LDLOC4 	-> 2 datoshi
    /// 27 : OpCode.INC 	-> 4 datoshi
    /// 28 : OpCode.STLOC4 	-> 2 datoshi
    /// 29 : OpCode.LDLOC4 	-> 2 datoshi
    /// 2A : OpCode.LDLOC3 	-> 2 datoshi
    /// 2B : OpCode.JMPLT F4 	-> 2 datoshi
    /// 2D : OpCode.LDLOC1 	-> 2 datoshi
    /// 2E : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADGhpagxkZWYMYWJjE8BwDHFoSnLKcxB0Ig9qbM51aW2L2yhxbJx0bGsw8WlA
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 68696A 	-> 8 datoshi
    /// 08 : OpCode.PUSHDATA1 646566 	-> 8 datoshi
    /// 0D : OpCode.PUSHDATA1 616263 	-> 8 datoshi
    /// 12 : OpCode.PUSH3 	-> 1 datoshi
    /// 13 : OpCode.PACK 	-> 2048 datoshi
    /// 14 : OpCode.STLOC0 	-> 2 datoshi
    /// 15 : OpCode.PUSHDATA1 	-> 8 datoshi
    /// 17 : OpCode.STLOC1 	-> 2 datoshi
    /// 18 : OpCode.LDLOC0 	-> 2 datoshi
    /// 19 : OpCode.DUP 	-> 2 datoshi
    /// 1A : OpCode.STLOC2 	-> 2 datoshi
    /// 1B : OpCode.SIZE 	-> 4 datoshi
    /// 1C : OpCode.STLOC3 	-> 2 datoshi
    /// 1D : OpCode.PUSH0 	-> 1 datoshi
    /// 1E : OpCode.STLOC4 	-> 2 datoshi
    /// 1F : OpCode.JMP 0F 	-> 2 datoshi
    /// 21 : OpCode.LDLOC2 	-> 2 datoshi
    /// 22 : OpCode.LDLOC4 	-> 2 datoshi
    /// 23 : OpCode.PICKITEM 	-> 64 datoshi
    /// 24 : OpCode.STLOC5 	-> 2 datoshi
    /// 25 : OpCode.LDLOC1 	-> 2 datoshi
    /// 26 : OpCode.LDLOC5 	-> 2 datoshi
    /// 27 : OpCode.CAT 	-> 2048 datoshi
    /// 28 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 2A : OpCode.STLOC1 	-> 2 datoshi
    /// 2B : OpCode.LDLOC4 	-> 2 datoshi
    /// 2C : OpCode.INC 	-> 4 datoshi
    /// 2D : OpCode.STLOC4 	-> 2 datoshi
    /// 2E : OpCode.LDLOC4 	-> 2 datoshi
    /// 2F : OpCode.LDLOC3 	-> 2 datoshi
    /// 30 : OpCode.JMPLT F1 	-> 2 datoshi
    /// 32 : OpCode.LDLOC1 	-> 2 datoshi
    /// 33 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwgAxUoLz0oQz3AMdGVzdDFKaBBR0EURSmgRUdBFxUoLz0oQz3EMdGVzdDJKaRBR0EUSSmkRUdBFaWgSwHLIc2pKdMp1EHYiF2xuzncHbwcRzkpvBxDOa1PQRW6cdm5tMOlrQA==
    /// 00 : OpCode.INITSLOT 0800 	-> 64 datoshi
    /// 03 : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 06 : OpCode.APPEND 	-> 8192 datoshi
    /// 07 : OpCode.DUP 	-> 2 datoshi
    /// 08 : OpCode.PUSH0 	-> 1 datoshi
    /// 09 : OpCode.APPEND 	-> 8192 datoshi
    /// 0A : OpCode.STLOC0 	-> 2 datoshi
    /// 0B : OpCode.PUSHDATA1 7465737431 	-> 8 datoshi
    /// 12 : OpCode.DUP 	-> 2 datoshi
    /// 13 : OpCode.LDLOC0 	-> 2 datoshi
    /// 14 : OpCode.PUSH0 	-> 1 datoshi
    /// 15 : OpCode.ROT 	-> 2 datoshi
    /// 16 : OpCode.SETITEM 	-> 8192 datoshi
    /// 17 : OpCode.DROP 	-> 2 datoshi
    /// 18 : OpCode.PUSH1 	-> 1 datoshi
    /// 19 : OpCode.DUP 	-> 2 datoshi
    /// 1A : OpCode.LDLOC0 	-> 2 datoshi
    /// 1B : OpCode.PUSH1 	-> 1 datoshi
    /// 1C : OpCode.ROT 	-> 2 datoshi
    /// 1D : OpCode.SETITEM 	-> 8192 datoshi
    /// 1E : OpCode.DROP 	-> 2 datoshi
    /// 1F : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 20 : OpCode.DUP 	-> 2 datoshi
    /// 21 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 22 : OpCode.APPEND 	-> 8192 datoshi
    /// 23 : OpCode.DUP 	-> 2 datoshi
    /// 24 : OpCode.PUSH0 	-> 1 datoshi
    /// 25 : OpCode.APPEND 	-> 8192 datoshi
    /// 26 : OpCode.STLOC1 	-> 2 datoshi
    /// 27 : OpCode.PUSHDATA1 7465737432 	-> 8 datoshi
    /// 2E : OpCode.DUP 	-> 2 datoshi
    /// 2F : OpCode.LDLOC1 	-> 2 datoshi
    /// 30 : OpCode.PUSH0 	-> 1 datoshi
    /// 31 : OpCode.ROT 	-> 2 datoshi
    /// 32 : OpCode.SETITEM 	-> 8192 datoshi
    /// 33 : OpCode.DROP 	-> 2 datoshi
    /// 34 : OpCode.PUSH2 	-> 1 datoshi
    /// 35 : OpCode.DUP 	-> 2 datoshi
    /// 36 : OpCode.LDLOC1 	-> 2 datoshi
    /// 37 : OpCode.PUSH1 	-> 1 datoshi
    /// 38 : OpCode.ROT 	-> 2 datoshi
    /// 39 : OpCode.SETITEM 	-> 8192 datoshi
    /// 3A : OpCode.DROP 	-> 2 datoshi
    /// 3B : OpCode.LDLOC1 	-> 2 datoshi
    /// 3C : OpCode.LDLOC0 	-> 2 datoshi
    /// 3D : OpCode.PUSH2 	-> 1 datoshi
    /// 3E : OpCode.PACK 	-> 2048 datoshi
    /// 3F : OpCode.STLOC2 	-> 2 datoshi
    /// 40 : OpCode.NEWMAP 	-> 8 datoshi
    /// 41 : OpCode.STLOC3 	-> 2 datoshi
    /// 42 : OpCode.LDLOC2 	-> 2 datoshi
    /// 43 : OpCode.DUP 	-> 2 datoshi
    /// 44 : OpCode.STLOC4 	-> 2 datoshi
    /// 45 : OpCode.SIZE 	-> 4 datoshi
    /// 46 : OpCode.STLOC5 	-> 2 datoshi
    /// 47 : OpCode.PUSH0 	-> 1 datoshi
    /// 48 : OpCode.STLOC6 	-> 2 datoshi
    /// 49 : OpCode.JMP 17 	-> 2 datoshi
    /// 4B : OpCode.LDLOC4 	-> 2 datoshi
    /// 4C : OpCode.LDLOC6 	-> 2 datoshi
    /// 4D : OpCode.PICKITEM 	-> 64 datoshi
    /// 4E : OpCode.STLOC 07 	-> 2 datoshi
    /// 50 : OpCode.LDLOC 07 	-> 2 datoshi
    /// 52 : OpCode.PUSH1 	-> 1 datoshi
    /// 53 : OpCode.PICKITEM 	-> 64 datoshi
    /// 54 : OpCode.DUP 	-> 2 datoshi
    /// 55 : OpCode.LDLOC 07 	-> 2 datoshi
    /// 57 : OpCode.PUSH0 	-> 1 datoshi
    /// 58 : OpCode.PICKITEM 	-> 64 datoshi
    /// 59 : OpCode.LDLOC3 	-> 2 datoshi
    /// 5A : OpCode.REVERSE3 	-> 2 datoshi
    /// 5B : OpCode.SETITEM 	-> 8192 datoshi
    /// 5C : OpCode.DROP 	-> 2 datoshi
    /// 5D : OpCode.LDLOC6 	-> 2 datoshi
    /// 5E : OpCode.INC 	-> 4 datoshi
    /// 5F : OpCode.STLOC6 	-> 2 datoshi
    /// 60 : OpCode.LDLOC6 	-> 2 datoshi
    /// 61 : OpCode.LDLOC5 	-> 2 datoshi
    /// 62 : OpCode.JMPLT E9 	-> 2 datoshi
    /// 64 : OpCode.LDLOC3 	-> 2 datoshi
    /// 65 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFRQTEhEVwHAQcTtUAGhKcspzEHQiRGpsznVtEqIQlyYEIjRpbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWycdGxrMLw9BXI9AmlA
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSH5 	-> 1 datoshi
    /// 04 : OpCode.PUSH4 	-> 1 datoshi
    /// 05 : OpCode.PUSH3 	-> 1 datoshi
    /// 06 : OpCode.PUSH2 	-> 1 datoshi
    /// 07 : OpCode.PUSH1 	-> 1 datoshi
    /// 08 : OpCode.PUSH5 	-> 1 datoshi
    /// 09 : OpCode.PACK 	-> 2048 datoshi
    /// 0A : OpCode.STLOC0 	-> 2 datoshi
    /// 0B : OpCode.PUSH0 	-> 1 datoshi
    /// 0C : OpCode.STLOC1 	-> 2 datoshi
    /// 0D : OpCode.TRY 5400 	-> 4 datoshi
    /// 10 : OpCode.LDLOC0 	-> 2 datoshi
    /// 11 : OpCode.DUP 	-> 2 datoshi
    /// 12 : OpCode.STLOC2 	-> 2 datoshi
    /// 13 : OpCode.SIZE 	-> 4 datoshi
    /// 14 : OpCode.STLOC3 	-> 2 datoshi
    /// 15 : OpCode.PUSH0 	-> 1 datoshi
    /// 16 : OpCode.STLOC4 	-> 2 datoshi
    /// 17 : OpCode.JMP 44 	-> 2 datoshi
    /// 19 : OpCode.LDLOC2 	-> 2 datoshi
    /// 1A : OpCode.LDLOC4 	-> 2 datoshi
    /// 1B : OpCode.PICKITEM 	-> 64 datoshi
    /// 1C : OpCode.STLOC5 	-> 2 datoshi
    /// 1D : OpCode.LDLOC5 	-> 2 datoshi
    /// 1E : OpCode.PUSH2 	-> 1 datoshi
    /// 1F : OpCode.MOD 	-> 8 datoshi
    /// 20 : OpCode.PUSH0 	-> 1 datoshi
    /// 21 : OpCode.EQUAL 	-> 32 datoshi
    /// 22 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 24 : OpCode.JMP 34 	-> 2 datoshi
    /// 26 : OpCode.LDLOC1 	-> 2 datoshi
    /// 27 : OpCode.LDLOC5 	-> 2 datoshi
    /// 28 : OpCode.ADD 	-> 8 datoshi
    /// 29 : OpCode.DUP 	-> 2 datoshi
    /// 2A : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 2F : OpCode.JMPGE 04 	-> 2 datoshi
    /// 31 : OpCode.JMP 0A 	-> 2 datoshi
    /// 33 : OpCode.DUP 	-> 2 datoshi
    /// 34 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 39 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 3B : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 44 : OpCode.AND 	-> 8 datoshi
    /// 45 : OpCode.DUP 	-> 2 datoshi
    /// 46 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 4B : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 4D : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 56 : OpCode.SUB 	-> 8 datoshi
    /// 57 : OpCode.STLOC1 	-> 2 datoshi
    /// 58 : OpCode.LDLOC4 	-> 2 datoshi
    /// 59 : OpCode.INC 	-> 4 datoshi
    /// 5A : OpCode.STLOC4 	-> 2 datoshi
    /// 5B : OpCode.LDLOC4 	-> 2 datoshi
    /// 5C : OpCode.LDLOC3 	-> 2 datoshi
    /// 5D : OpCode.JMPLT BC 	-> 2 datoshi
    /// 5F : OpCode.ENDTRY 05 	-> 4 datoshi
    /// 61 : OpCode.STLOC2 	-> 2 datoshi
    /// 62 : OpCode.ENDTRY 02 	-> 4 datoshi
    /// 64 : OpCode.LDLOC1 	-> 2 datoshi
    /// 65 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoNwAAQc/nR5ZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoFbUkwUA=
    /// 00 : OpCode.INITSLOT 0100 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 09 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 0E : OpCode.LDLOC0 	-> 2 datoshi
    /// 0F : OpCode.DUP 	-> 2 datoshi
    /// 10 : OpCode.INC 	-> 4 datoshi
    /// 11 : OpCode.DUP 	-> 2 datoshi
    /// 12 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 17 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 19 : OpCode.JMP 0A 	-> 2 datoshi
    /// 1B : OpCode.DUP 	-> 2 datoshi
    /// 1C : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 21 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 2C : OpCode.AND 	-> 8 datoshi
    /// 2D : OpCode.DUP 	-> 2 datoshi
    /// 2E : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 33 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 35 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 3E : OpCode.SUB 	-> 8 datoshi
    /// 3F : OpCode.STLOC0 	-> 2 datoshi
    /// 40 : OpCode.DROP 	-> 2 datoshi
    /// 41 : OpCode.LDLOC0 	-> 2 datoshi
    /// 42 : OpCode.PUSH5 	-> 1 datoshi
    /// 43 : OpCode.LT 	-> 8 datoshi
    /// 44 : OpCode.JMPIF C1 	-> 2 datoshi
    /// 46 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUANOZKcMpxEHIiHmhqzsFFc3RrNwAADDogi2yL2yhBz+dHlmqccmppMOJA
    /// 00 : OpCode.INITSLOT 0500 	-> 64 datoshi
    /// 03 : OpCode.CALL E6 	-> 512 datoshi
    /// 05 : OpCode.DUP 	-> 2 datoshi
    /// 06 : OpCode.STLOC0 	-> 2 datoshi
    /// 07 : OpCode.SIZE 	-> 4 datoshi
    /// 08 : OpCode.STLOC1 	-> 2 datoshi
    /// 09 : OpCode.PUSH0 	-> 1 datoshi
    /// 0A : OpCode.STLOC2 	-> 2 datoshi
    /// 0B : OpCode.JMP 1E 	-> 2 datoshi
    /// 0D : OpCode.LDLOC0 	-> 2 datoshi
    /// 0E : OpCode.LDLOC2 	-> 2 datoshi
    /// 0F : OpCode.PICKITEM 	-> 64 datoshi
    /// 10 : OpCode.UNPACK 	-> 2048 datoshi
    /// 11 : OpCode.DROP 	-> 2 datoshi
    /// 12 : OpCode.STLOC3 	-> 2 datoshi
    /// 13 : OpCode.STLOC4 	-> 2 datoshi
    /// 14 : OpCode.LDLOC3 	-> 2 datoshi
    /// 15 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 18 : OpCode.PUSHDATA1 3A20 	-> 8 datoshi
    /// 1C : OpCode.CAT 	-> 2048 datoshi
    /// 1D : OpCode.LDLOC4 	-> 2 datoshi
    /// 1E : OpCode.CAT 	-> 2048 datoshi
    /// 1F : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 21 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 26 : OpCode.LDLOC2 	-> 2 datoshi
    /// 27 : OpCode.INC 	-> 4 datoshi
    /// 28 : OpCode.STLOC2 	-> 2 datoshi
    /// 29 : OpCode.LDLOC2 	-> 2 datoshi
    /// 2A : OpCode.LDLOC1 	-> 2 datoshi
    /// 2B : OpCode.JMPLT E2 	-> 2 datoshi
    /// 2D : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAE0Gb9mfOExGIThBR0FASwMFFQd8wuJpwaHEiEWlB81S/HXJq2yhBz+dHlmlBnAjtnCTrQA==
    /// 00 : OpCode.INITSLOT 0300 	-> 64 datoshi
    /// 03 : OpCode.PUSH3 	-> 1 datoshi
    /// 04 : OpCode.SYSCALL 9BF667CE 	-> 0 datoshi
    /// 09 : OpCode.PUSH3 	-> 1 datoshi
    /// 0A : OpCode.PUSH1 	-> 1 datoshi
    /// 0B : OpCode.NEWBUFFER 	-> 256 datoshi
    /// 0C : OpCode.TUCK 	-> 2 datoshi
    /// 0D : OpCode.PUSH0 	-> 1 datoshi
    /// 0E : OpCode.ROT 	-> 2 datoshi
    /// 0F : OpCode.SETITEM 	-> 8192 datoshi
    /// 10 : OpCode.SWAP 	-> 2 datoshi
    /// 11 : OpCode.PUSH2 	-> 1 datoshi
    /// 12 : OpCode.PACK 	-> 2048 datoshi
    /// 13 : OpCode.UNPACK 	-> 2048 datoshi
    /// 14 : OpCode.DROP 	-> 2 datoshi
    /// 15 : OpCode.SYSCALL DF30B89A 	-> 0 datoshi
    /// 1A : OpCode.STLOC0 	-> 2 datoshi
    /// 1B : OpCode.LDLOC0 	-> 2 datoshi
    /// 1C : OpCode.STLOC1 	-> 2 datoshi
    /// 1D : OpCode.JMP 11 	-> 2 datoshi
    /// 1F : OpCode.LDLOC1 	-> 2 datoshi
    /// 20 : OpCode.SYSCALL F354BF1D 	-> 0 datoshi
    /// 25 : OpCode.STLOC2 	-> 2 datoshi
    /// 26 : OpCode.LDLOC2 	-> 2 datoshi
    /// 27 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 29 : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 2E : OpCode.LDLOC1 	-> 2 datoshi
    /// 2F : OpCode.SYSCALL 9C08ED9C 	-> 0 datoshi
    /// 34 : OpCode.JMPIF EB 	-> 2 datoshi
    /// 36 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoFbUmQGg3AABBz+dHlmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSK/QA==
    /// 00 : OpCode.INITSLOT 0100 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.PUSH5 	-> 1 datoshi
    /// 07 : OpCode.LT 	-> 8 datoshi
    /// 08 : OpCode.JMPIFNOT 40 	-> 2 datoshi
    /// 0A : OpCode.LDLOC0 	-> 2 datoshi
    /// 0B : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 0E : OpCode.SYSCALL CFE74796 	-> 0 datoshi
    /// 13 : OpCode.LDLOC0 	-> 2 datoshi
    /// 14 : OpCode.DUP 	-> 2 datoshi
    /// 15 : OpCode.INC 	-> 4 datoshi
    /// 16 : OpCode.DUP 	-> 2 datoshi
    /// 17 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 1C : OpCode.JMPGE 04 	-> 2 datoshi
    /// 1E : OpCode.JMP 0A 	-> 2 datoshi
    /// 20 : OpCode.DUP 	-> 2 datoshi
    /// 21 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 26 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 31 : OpCode.AND 	-> 8 datoshi
    /// 32 : OpCode.DUP 	-> 2 datoshi
    /// 33 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 38 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 3A : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 43 : OpCode.SUB 	-> 8 datoshi
    /// 44 : OpCode.STLOC0 	-> 2 datoshi
    /// 45 : OpCode.DROP 	-> 2 datoshi
    /// 46 : OpCode.JMP BF 	-> 2 datoshi
    /// 48 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 	-> 8 datoshi
    /// 19 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 	-> 8 datoshi
    /// 2F : OpCode.PUSH2 	-> 1 datoshi
    /// 30 : OpCode.PACK 	-> 2048 datoshi
    /// 31 : OpCode.STLOC0 	-> 2 datoshi
    /// 32 : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 33 : OpCode.STLOC1 	-> 2 datoshi
    /// 34 : OpCode.LDLOC0 	-> 2 datoshi
    /// 35 : OpCode.DUP 	-> 2 datoshi
    /// 36 : OpCode.STLOC2 	-> 2 datoshi
    /// 37 : OpCode.SIZE 	-> 4 datoshi
    /// 38 : OpCode.STLOC3 	-> 2 datoshi
    /// 39 : OpCode.PUSH0 	-> 1 datoshi
    /// 3A : OpCode.STLOC4 	-> 2 datoshi
    /// 3B : OpCode.JMP 0C 	-> 2 datoshi
    /// 3D : OpCode.LDLOC2 	-> 2 datoshi
    /// 3E : OpCode.LDLOC4 	-> 2 datoshi
    /// 3F : OpCode.PICKITEM 	-> 64 datoshi
    /// 40 : OpCode.STLOC5 	-> 2 datoshi
    /// 41 : OpCode.LDLOC1 	-> 2 datoshi
    /// 42 : OpCode.LDLOC5 	-> 2 datoshi
    /// 43 : OpCode.APPEND 	-> 8192 datoshi
    /// 44 : OpCode.LDLOC4 	-> 2 datoshi
    /// 45 : OpCode.INC 	-> 4 datoshi
    /// 46 : OpCode.STLOC4 	-> 2 datoshi
    /// 47 : OpCode.LDLOC4 	-> 2 datoshi
    /// 48 : OpCode.LDLOC3 	-> 2 datoshi
    /// 49 : OpCode.JMPLT F4 	-> 2 datoshi
    /// 4B : OpCode.LDLOC1 	-> 2 datoshi
    /// 4C : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 	-> 8 datoshi
    /// 25 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 	-> 8 datoshi
    /// 47 : OpCode.PUSH2 	-> 1 datoshi
    /// 48 : OpCode.PACK 	-> 2048 datoshi
    /// 49 : OpCode.STLOC0 	-> 2 datoshi
    /// 4A : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 4B : OpCode.STLOC1 	-> 2 datoshi
    /// 4C : OpCode.LDLOC0 	-> 2 datoshi
    /// 4D : OpCode.DUP 	-> 2 datoshi
    /// 4E : OpCode.STLOC2 	-> 2 datoshi
    /// 4F : OpCode.SIZE 	-> 4 datoshi
    /// 50 : OpCode.STLOC3 	-> 2 datoshi
    /// 51 : OpCode.PUSH0 	-> 1 datoshi
    /// 52 : OpCode.STLOC4 	-> 2 datoshi
    /// 53 : OpCode.JMP 0C 	-> 2 datoshi
    /// 55 : OpCode.LDLOC2 	-> 2 datoshi
    /// 56 : OpCode.LDLOC4 	-> 2 datoshi
    /// 57 : OpCode.PICKITEM 	-> 64 datoshi
    /// 58 : OpCode.STLOC5 	-> 2 datoshi
    /// 59 : OpCode.LDLOC1 	-> 2 datoshi
    /// 5A : OpCode.LDLOC5 	-> 2 datoshi
    /// 5B : OpCode.APPEND 	-> 8192 datoshi
    /// 5C : OpCode.LDLOC4 	-> 2 datoshi
    /// 5D : OpCode.INC 	-> 4 datoshi
    /// 5E : OpCode.STLOC4 	-> 2 datoshi
    /// 5F : OpCode.LDLOC4 	-> 2 datoshi
    /// 60 : OpCode.LDLOC3 	-> 2 datoshi
    /// 61 : OpCode.JMPLT F4 	-> 2 datoshi
    /// 63 : OpCode.LDLOC1 	-> 2 datoshi
    /// 64 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion
}
