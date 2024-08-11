using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_String : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_String"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testEqual"",""parameters"":[],""returntype"":""Void"",""offset"":82,""safe"":false},{""name"":""testSubstring"",""parameters"":[],""returntype"":""Void"",""offset"":127,""safe"":false},{""name"":""testEmpty"",""parameters"":[],""returntype"":""String"",""offset"":163,""safe"":false},{""name"":""testIsNullOrEmpty"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":166,""safe"":false},{""name"":""testStringNull"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Integer"",""offset"":182,""safe"":false},{""name"":""testEndWith"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":188,""safe"":false},{""name"":""testContains"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":228,""safe"":false},{""name"":""testIndexOf"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Integer"",""offset"":245,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa"",""memorySearch""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ++8gQxQDYqd8FQmcfmTBL3ALZl2gtjdXJyZW50SGFzaAAAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAIAAQ8AAP0EAVcDAAwETWFya3AMAHE3AQA3AAAUznIMB0hlbGxvLCBoiwwBIItpiwwXISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMAS6L2yhBz+dHlkBXAgAMBWhlbGxvcAwFaGVsbG9xaGmXJAsMBUZhbHNlIggMBFRydWVBz+dHlkBXAQAMCDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkAMAEBXAAF4StgkB8oQsyIERQhAVwABeMpAVwABDAV3b3JsZHhKylFKykoTUlCfShAsCUVFRUUJIgkTUlOM2yiXQFcAAQwFd29ybGR4NwMAELhAVwABDAV3b3JsZHg3AwBAY11VAw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContains")]
    public abstract bool TestContains(string str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEmpty")]
    public abstract string TestEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEndWith")]
    public abstract bool TestEndWith(string str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexOf")]
    public abstract BigInteger TestIndexOf(string str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool TestIsNullOrEmpty(string str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringNull")]
    public abstract BigInteger TestStringNull(string str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    #endregion

    #region Constructor for internal use only

    protected Contract_String(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
