using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract1 : SmartContract.Framework.SmartContract
    {
        private static string privateMethod()
        {
            BigInteger a = 0;
            var b = a * 1000;
            var c = a * 1_000_000;
            var d = a * 100_000_000;

            return "NEO3" + b + c + d;
        }

        public static byte[] unitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

        public static void testVoid()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
        }

        public static byte[] testArgs1(byte a)
        {
            var nb = new byte[] { 1, 2, 3, 3 };
            nb[3] = a;
            return nb;
        }

        public static object testArgs2(byte[] a)
        {
            BigInteger aa = 0;
            var b = aa * 1000;
            var c = aa * 1_000_000;
            var d = aa * 100_000_000;

            return a;
        }

        public static void testArgs3(int a, int b)
        {
            a = a + 2;
        }

        public static int testArgs4(int a, int b)
        {
            a = a + 2;
            return a + b;
        }
    }
}
