using System.Linq;

namespace BlockChain.CLI.Bitcoin
{
    public static class NetworkVersion
    {
        // From https://en.bitcoin.it/wiki/List_of_address_prefixes

        private static readonly byte[] Unknown = { 0xFF };
        private static readonly byte[] MainNetworkPubKey = { 0x00 };
        private static readonly byte[] MainNetworkScript = { 0x05 };
        private static readonly byte[] MainNetworkPrivKey = { 0x80 };
        private static readonly byte[] TestNetworkPubKey = { 0x6F };
        private static readonly byte[] TestNetworkScript = { 0xC4 };
        private static readonly byte[] TestNetworkPrivKey = { 0xEF };
        private static readonly byte[] MainNetworkBip32PubKey = { 0x04, 0x88, 0xB2, 0x1E };
        private static readonly byte[] MainNetworkBip32PrivKey = { 0x04, 0x88, 0xAD, 0xE4 };
        private static readonly byte[] TestNetworkBip32PubKey = { 0x04, 0x35, 0x87, 0xCF };
        private static readonly byte[] TestNetworkBip32PrivKey = { 0x04, 0x35, 0x83, 0x94 };
        private static readonly byte[] MainNetworkWitnessPubKey = { 0x06 };
        private static readonly byte[] MainNetworkWitnessScript = { 0x0A };
        private static readonly byte[] TestNetworkWitnessPubKey = { 0x03 };
        private static readonly byte[] TestNetworkWitnessScript = { 0x28 };

        public enum Type
        {
            Unknown = -1,
            PublicKey,
            PrivateKey,
            Script,
            Bip32PublicKey,
            Bip32PrivateKey,
            WitnessPublicKey,
            WitnessScript
        }

        public enum Network
        {
            Unknown = -1,
            Main,
            Test,
            SegNet = Test
        }

        public static byte[] GetPrefix(this (Type type, Network network) type)
        {
            switch (type.network)
            {
                case Network.Main:
                    switch (type.type)
                    {
                        case Type.PublicKey:
                            return MainNetworkPubKey;
                        case Type.PrivateKey:
                            return MainNetworkPrivKey;
                        case Type.Script:
                            return MainNetworkScript;
                        case Type.Bip32PublicKey:
                            return MainNetworkBip32PubKey;
                        case Type.Bip32PrivateKey:
                            return MainNetworkBip32PrivKey;
                        case Type.WitnessPublicKey:
                            return MainNetworkWitnessPubKey;
                        case Type.WitnessScript:
                            return MainNetworkWitnessScript;
                        default:
                            return new byte[] { };
                    }
                case Network.Test:
                    switch (type.type)
                    {
                        case Type.PublicKey:
                            return TestNetworkPubKey;
                        case Type.PrivateKey:
                            return TestNetworkPrivKey;
                        case Type.Script:
                            return TestNetworkScript;
                        case Type.Bip32PublicKey:
                            return TestNetworkBip32PubKey;
                        case Type.Bip32PrivateKey:
                            return TestNetworkBip32PrivKey;
                        case Type.WitnessPublicKey:
                            return TestNetworkWitnessPubKey;
                        case Type.WitnessScript:
                            return TestNetworkWitnessScript;
                        default:
                            return new byte[] { };
                    }
                default:
                    return new byte[] { };
            }
        }

        public static (Type type, Network network) GetNetworkVersion(this byte[] address)
        {
            byte headerByte = address[0];

            switch (headerByte)
            {
                case 0x00:
                    return (Type.PublicKey, Network.Main);
                case 0x03:
                    return (Type.WitnessPublicKey, Network.Test);
                case 0x04:
                    {
                        var header = address.SubArray(4);

                        if (header.SequenceEqual(MainNetworkBip32PubKey))
                            return (Type.Bip32PublicKey, Network.Main);
                        if (header.SequenceEqual(MainNetworkBip32PrivKey))
                            return (Type.Bip32PrivateKey, Network.Main);
                        if (header.SequenceEqual(TestNetworkBip32PubKey))
                            return (Type.Bip32PublicKey, Network.Test);
                        if (header.SequenceEqual(TestNetworkBip32PrivKey))
                            return (Type.Bip32PrivateKey, Network.Test);
                    }
                    return (Type.Unknown, Network.Unknown);
                case 0x05:
                    return (Type.Script, Network.Main);
                case 0x06:
                    return (Type.WitnessPublicKey, Network.Main);
                case 0x0A:
                    return (Type.WitnessScript, Network.Main);
                case 0x28:
                    return (Type.WitnessScript, Network.Test);
                case 0x6F:
                    return (Type.PublicKey, Network.Test);
                case 0x80:
                    return (Type.PrivateKey, Network.Main);
                case 0xC4:
                    return (Type.Script, Network.Test);
                case 0xEF:
                    return (Type.PrivateKey, Network.Test);
                default:
                    return (Type.Unknown, Network.Unknown);
            }
        }
    }
}
