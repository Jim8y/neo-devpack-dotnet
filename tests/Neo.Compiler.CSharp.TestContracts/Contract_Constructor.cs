using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    [SupportedStandards(NepStandard.Nep11)]
    public class Contract_Constructor : Nep11Token<MyTokenState>
    {
        //TODO: Replace it with your own address.
        static readonly UInt160 Owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB";

        private static bool IsOwner() => Runtime.CheckWitness(Owner);

        public override string Symbol { [Safe] get => "MNFT"; }

        public static bool Airdrop(string name)
        {
            var nft = new MyTokenState(name);
            return true;
        }
    }

    public class MyTokenState(string name) : Nep11TokenState
    {
        public string Image { get; set; } = $"https://neo.org/images/{name}.jpg";
    }
}
