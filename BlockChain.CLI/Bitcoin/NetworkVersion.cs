using System.Linq;

namespace BlockChain.CLI.Bitcoin
{
    public static class NetworkVersion
    {
        // From https://en.bitcoin.it/wiki/List_of_address_prefixes

        private static readonly byte[] MainNetworkPubKey = { 0x00 };
        private static readonly byte[] MainNetworkPrivKey = { 0x80 };
        private static readonly byte[] MainNetworkScript = { 0x05 };
        private static readonly byte[] TestNetworkPubKey = { 0x6F };
        private static readonly byte[] TestNetworkScript = { 0xC4 };
        private static readonly byte[] TestNetworkPrivKey = { 0xEF };
        private static readonly byte[] MainNetworkBip32PubKey = { 0x04, 0x88, 0xB2, 0x1E };
        private static readonly byte[] MainNetworkBip32PrivKey = { 0x04, 0x88, 0xAD, 0xE4 };
        private static readonly byte[] TestNetworkBip32PubKey = { 0x04, 0x35, 0x87, 0xCF };
        private static readonly byte[] TestNetworkBip32PrivKey = { 0x04, 0x35, 0x83, 0x94 };

        public enum Type
        {
            Unknown = -1,
            MainNetworkPubKey,
            MainNetworkPrivKey,
            MainNetworkScript,
            TestNetworkPubKey,
            TestNetworkScript,
            TestNetworkPrivKey,
            MainNetworkBip32PubKey,
            MainNetworkBip32PrivKey,
            TestNetworkBip32PubKey,
            TestNetworkBip32PrivKey
        }

        public static byte[] GetPrefix(this Type type)
        {
            switch (type)
            {
                case Type.MainNetworkPubKey:
                    return MainNetworkPubKey;
                case Type.MainNetworkPrivKey:
                    return MainNetworkPrivKey;
                case Type.MainNetworkScript:
                    return MainNetworkScript;
                case Type.TestNetworkPubKey:
                    return TestNetworkPubKey;
                case Type.TestNetworkScript:
                    return TestNetworkScript;
                case Type.TestNetworkPrivKey:
                    return TestNetworkPrivKey;
                case Type.MainNetworkBip32PubKey:
                    return MainNetworkBip32PubKey;
                case Type.MainNetworkBip32PrivKey:
                    return MainNetworkBip32PrivKey;
                case Type.TestNetworkBip32PubKey:
                    return TestNetworkBip32PubKey;
                case Type.TestNetworkBip32PrivKey:
                    return TestNetworkBip32PrivKey;
                default:
                    return new byte[] { };
            }
        }

        public static Type GetNetworkType(this byte[] address)
        {
            byte headerByte = address[0];

            switch (headerByte)
            {
                case 0x00:
                    return Type.MainNetworkPubKey;
                case 0x80:
                    return Type.MainNetworkPrivKey;
                case 0x05:
                    return Type.MainNetworkScript;
                case 0x6F:
                    return Type.TestNetworkPubKey;
                case 0xC4:
                    return Type.TestNetworkScript;
                case 0xEF:
                    return Type.TestNetworkPrivKey;
                case 0x04:
                    {
                        var header = ArrayHelpers.SubArray(address, 4);

                        if (header.SequenceEqual(MainNetworkBip32PubKey))
                            return Type.MainNetworkBip32PubKey;
                        if (header.SequenceEqual(MainNetworkBip32PrivKey))
                            return Type.MainNetworkBip32PrivKey;
                        if (header.SequenceEqual(TestNetworkBip32PubKey))
                            return Type.TestNetworkBip32PubKey;
                        if (header.SequenceEqual(TestNetworkBip32PrivKey))
                            return Type.TestNetworkBip32PrivKey;
                    }
                    return Type.Unknown;
                default:
                    return Type.Unknown;
            }
        }
    }
}
