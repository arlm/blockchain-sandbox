using System;
using System.Linq;
using BlockChain.CLI.Bitcoin.Core;
using BlockChain.CLI.Core;
using BlockChain.CLI.Core.Interfaces;

namespace BlockChain.CLI.Bitcoin
{
    public struct NetworkVersion : INetworkVersion<AddressType, NetworkType>,
        IEquatable<AddressType>, IEquatable<NetworkType>,
        IComparable<NetworkVersion>, IEquatable<NetworkVersion>
    {
        // From https://en.bitcoin.it/wiki/List_of_address_prefixes

        private static readonly byte[] UnknownPrefix = { 0xFF };
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

        public static readonly NetworkVersion Unknown = new NetworkVersion(AddressType.Unknown, NetworkType.Unknown);

        private readonly NetworkType network;
        public NetworkType Network => network;

        private readonly AddressType type;
        public AddressType Type => type;

        public NetworkVersion(AddressType type, NetworkType network) : this()
        {
            this.type = type;
            this.network = network;
        }

        public byte[] Prefix
        {
            get
            {
                switch (network)
                {
                    case NetworkType.Main:
                        switch (type)
                        {
                            case AddressType.PublicKey:
                                return MainNetworkPubKey;
                            case AddressType.PrivateKey:
                                return MainNetworkPrivKey;
                            case AddressType.Script:
                                return MainNetworkScript;
                            case AddressType.Bip32PublicKey:
                                return MainNetworkBip32PubKey;
                            case AddressType.Bip32PrivateKey:
                                return MainNetworkBip32PrivKey;
                            case AddressType.WitnessPublicKey:
                                return MainNetworkWitnessPubKey;
                            case AddressType.WitnessScript:
                                return MainNetworkWitnessScript;
                            default:
                                return new byte[] { };
                        }
                    case NetworkType.Test:
                        switch (type)
                        {
                            case AddressType.PublicKey:
                                return TestNetworkPubKey;
                            case AddressType.PrivateKey:
                                return TestNetworkPrivKey;
                            case AddressType.Script:
                                return TestNetworkScript;
                            case AddressType.Bip32PublicKey:
                                return TestNetworkBip32PubKey;
                            case AddressType.Bip32PrivateKey:
                                return TestNetworkBip32PrivKey;
                            case AddressType.WitnessPublicKey:
                                return TestNetworkWitnessPubKey;
                            case AddressType.WitnessScript:
                                return TestNetworkWitnessScript;
                            default:
                                return new byte[] { };
                        }
                    default:
                        return new byte[] { };
                }
            }
        }

        public AddressBase<AddressType, NetworkType> Create(string address)
        {
            if (network == NetworkType.Unknown)
            {
                throw new InvalidOperationException("Unknown network type");
            }

            var version = Parse(address);

            switch (version.Type)
            {
                case AddressType.PublicKey:
                    return new PublicAddress(address);
                case AddressType.PrivateKey:
                    return new PrivateAddress(address);
                case AddressType.Script:
                    return new Script(address);
                case AddressType.Bip32PublicKey:
                    return new DeterministicWallet.PublicAddress(address);
                case AddressType.Bip32PrivateKey:
                    return new DeterministicWallet.PrivateAddress(address);
                case AddressType.WitnessPublicKey:
                    return new SegregatedWitness.PublicAddress(address);
                case AddressType.WitnessScript:
                    return new SegregatedWitness.Script(address);
                default:
                    throw new InvalidOperationException("Unknown address type");
            }
        }

        public AddressBase<AddressType, NetworkType> Create(byte[] key, bool compressedPubKey = false)
        {
            if (network == NetworkType.Unknown)
            {
                throw new InvalidOperationException("Unknown network type");
            }

            switch (type)
            {
                case AddressType.PublicKey:
                    return new PublicAddress(key, Network);
                case AddressType.PrivateKey:
                    return new PrivateAddress(key, Network, compressedPubKey);
                case AddressType.Script:
                    return new Script(key, Network);
                case AddressType.Bip32PublicKey:
                    return new DeterministicWallet.PublicAddress(key, Network);
                case AddressType.Bip32PrivateKey:
                    return new DeterministicWallet.PrivateAddress(key, Network, compressedPubKey);
                case AddressType.WitnessPublicKey:
                    return new SegregatedWitness.PublicAddress(key, Network);
                case AddressType.WitnessScript:
                    return new SegregatedWitness.Script(key, Network);
                default:
                    throw new InvalidOperationException("Unknown address type");
            }
        }

        public static AddressBase<AddressType, NetworkType> CreateFromAddress(string address)
        {
            var version = Parse(address);

            return version.Create(address);
        }

        public static AddressBase<AddressType, NetworkType> CreateFromAddress(byte[] data)
        {
            var version = Parse(data);

            return version.Create(data.EncodeBase58());
        }

        public static NetworkVersion Parse(string address)
        {
            byte[] data;

            data = address.DecodeBase58();

            if (data == null || data.Length == 0)
            {
                data = address.DecodeBase64();
            }

            return Parse(data);
        }

        public static NetworkVersion Parse(byte[] data)
        {
            byte headerByte = data[0];

            switch (headerByte)
            {
                case 0x00:
                    return new NetworkVersion(AddressType.PublicKey, NetworkType.Main);
                case 0x03:
                    return new NetworkVersion(AddressType.WitnessPublicKey, NetworkType.Test);
                case 0x04:
                    {
                        var header = data.SubArray(4);

                        if (header.SequenceEqual(MainNetworkBip32PubKey))
                            return new NetworkVersion(AddressType.Bip32PublicKey, NetworkType.Main);
                        if (header.SequenceEqual(MainNetworkBip32PrivKey))
                            return new NetworkVersion(AddressType.Bip32PrivateKey, NetworkType.Main);
                        if (header.SequenceEqual(TestNetworkBip32PubKey))
                            return new NetworkVersion(AddressType.Bip32PublicKey, NetworkType.Test);
                        if (header.SequenceEqual(TestNetworkBip32PrivKey))
                            return new NetworkVersion(AddressType.Bip32PrivateKey, NetworkType.Test);
                    }
                    return new NetworkVersion(AddressType.Unknown, NetworkType.Unknown);
                case 0x05:
                    return new NetworkVersion(AddressType.Script, NetworkType.Main);
                case 0x06:
                    return new NetworkVersion(AddressType.WitnessPublicKey, NetworkType.Main);
                case 0x0A:
                    return new NetworkVersion(AddressType.WitnessScript, NetworkType.Main);
                case 0x28:
                    return new NetworkVersion(AddressType.WitnessScript, NetworkType.Test);
                case 0x6F:
                    return new NetworkVersion(AddressType.PublicKey, NetworkType.Test);
                case 0x80:
                    return new NetworkVersion(AddressType.PrivateKey, NetworkType.Main);
                case 0xC4:
                    return new NetworkVersion(AddressType.Script, NetworkType.Test);
                case 0xEF:
                    return new NetworkVersion(AddressType.PrivateKey, NetworkType.Test);
                default:
                    return new NetworkVersion(AddressType.Unknown, NetworkType.Unknown);
            }
        }

        public override bool Equals(object obj)
        {
            var version = obj as NetworkVersion?;

            if (version.HasValue)
            {
                return this.Equals(version.Value);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(NetworkVersion other)
        {
            return this.type == other.type && this.network == other.network;
        }


        public bool Equals(INetworkVersion<AddressType, NetworkType> other)
        {
            return this.type == other.Type && this.network == other.Network;
        }

        public bool Equals(AddressType other)
        {
            return this.type == other;
        }

        public bool Equals(NetworkType other)
        {
            return this.network == other;
        }

        public int CompareTo(NetworkVersion other)
        {
            return this.type.CompareTo(other.type) + this.network.CompareTo(other.network);
        }

        public int CompareTo(INetworkVersion<AddressType, NetworkType> other)
        {
            return this.type.CompareTo(other.Type) + this.network.CompareTo(other.Network);
        }

        public int CompareTo(object obj)
        {
            var version = obj as NetworkVersion?;

            if (version.HasValue)
            {
                return this.CompareTo(version.Value);
            }
            else
            {
                return -1;
            }
        }

        public static bool operator ==(NetworkVersion obj, NetworkType network) => obj.network == network;
        public static bool operator !=(NetworkVersion obj, NetworkType network) => !(obj == network);
        public static bool operator ==(NetworkVersion obj, AddressType type) => obj.type == type;
        public static bool operator !=(NetworkVersion obj, AddressType type) => !(obj == type);
        public static bool operator ==(NetworkVersion obj, NetworkVersion other) => obj.network == other.network && obj.type == other.type;
        public static bool operator !=(NetworkVersion obj, NetworkVersion other) => !(obj == other);

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ network.GetHashCode();
        }

        public override string ToString() => $"[Address Type = {type:G}, Network = {network:G}]";
    }
}
