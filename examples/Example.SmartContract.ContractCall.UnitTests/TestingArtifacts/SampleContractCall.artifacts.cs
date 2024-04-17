using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleContractCall : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Integer""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":91,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample contract to demonstrate how to call a contract"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIrZmFiMWEyZWVhZGYyMTE2NjhiMjg0ZWZiYTgwYzFhNTU3ZTYuLi4AAAAAAHVXAwN6AHuzqiYEIj5B2/6odHBBOVNuPHFoEcAfDAliYWxhbmNlT2ZpQWJ9W1JyamloE8AfDAtkdW1teU1ldGhvZFhBYn1bUkVAQdv+qHRAQTlTbjxAQWJ9W1JAVgEMFF6QEGiCvFczbXZ7FdXtLpwFPqgTYECNxQkQ"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, BigInteger? data);

    #endregion

    #region Constructor for internal use only

    protected SampleContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
